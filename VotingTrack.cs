using AssettoServer.Network.Tcp;
using AssettoServer.Server;
using AssettoServer.Server.Plugin;
using AssettoServer.Shared.Network.Packets.Shared;
using AssettoServer.Shared.Services;
using Microsoft.Extensions.Hosting;
using Serilog;
using VotingTrackPlugin.Track;

namespace VotingTrackPlugin;

public class VotingTrack : CriticalBackgroundService, IAssettoServerAutostart
{
    private readonly EntryCarManager _entryCarManager;
    private readonly TrackManager _trackManager;
    private readonly VotingTrackConfiguration _configuration;
    private readonly List<ACTcpClient> _alreadyVoted = new();
    private readonly List<TrackChoice> _availableTracks = new();
    private readonly List<TrackType> _tracks;

    private bool _votingOpen = false;
    private bool _adminTrackChange = false;
    private TrackData? _adminTrack = null;

    private class TrackChoice
    {
        public TrackType Track { get; init; }
        public int Votes { get; set; }
    }

    public VotingTrack(VotingTrackConfiguration configuration, EntryCarManager entryCarManager, TrackManager trackManager, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _configuration = configuration;
        _entryCarManager = entryCarManager;
        _trackManager = trackManager;
        
        _tracks = _configuration.AvailableTracks;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => ExecuteAdminAsync(stoppingToken), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_configuration.VotingIntervalMilliseconds - _configuration.VotingDurationMilliseconds, stoppingToken);
            try
            {
                await UpdateAsync(stoppingToken);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during voting track update");
            }
            // finally { }
        }
    }

    internal void ListAllTracks(ACTcpClient client)
    {
        client.SendPacket(new ChatMessage { SessionId = 255, Message = "Vote for next track:" });
        for (int i = 0; i < _tracks.Count; i++)
        {
            var track = _tracks[i];
            client.SendPacket(new ChatMessage { SessionId = 255, Message = $" /admintrackset {i} - {track.Name}" });
        }
    }
    internal void SetTrack(ACTcpClient client, int choice)
    {
        var last = _trackManager.CurrentTrack;
        
        if (choice < 0 && choice >= _tracks.Count)
        {
            client.SendPacket(new ChatMessage { SessionId = 255, Message = "Invalid track choice." });

            return;
        }
        var next = _tracks[choice];
        
        if (last.Type == next)
        {
            client.SendPacket(new ChatMessage { SessionId = 255, Message = $"No change made, you tried setting the current track." });
        }
        else
        {
            _adminTrack = new TrackData(_trackManager.CurrentTrack.Type, next)
            {
                TransitionDuration = _configuration.TransitionDurationMilliseconds,
                UpdateContentManager = _configuration.UpdateContentManager
            };
            _adminTrackChange = true;
        }
    }
    
    internal void CountVote(ACTcpClient client, int choice)
    {
        if (!_votingOpen)
        {
            client.SendPacket(new ChatMessage { SessionId = 255, Message = "There is no ongoing track vote." });
            return;
        }

        if (choice >= _configuration.NumChoices || choice < 0)
        {
            client.SendPacket(new ChatMessage { SessionId = 255, Message = "Invalid choice." });
            return;
        }

        if (_alreadyVoted.Contains(client))
        {
            client.SendPacket(new ChatMessage { SessionId = 255, Message = "You voted already." });
            return;
        }

        _alreadyVoted.Add(client);

        var votedTrack = _availableTracks[choice];
        votedTrack.Votes++;

        client.SendPacket(new ChatMessage { SessionId = 255, Message = $"Your vote for {votedTrack.Track.Name} has been counted." });
    }

    private async Task UpdateAsync(CancellationToken stoppingToken)
    {
        var last = _trackManager.CurrentTrack;
        
        _availableTracks.Clear();
        _alreadyVoted.Clear();

        var tracksLeft = new List<TrackType>(_tracks);

        _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = "Vote for next track:" });
        for (int i = 0; i < _configuration.NumChoices; i++)
        {
            var nextTrack = tracksLeft[Random.Shared.Next(tracksLeft.Count)];
            _availableTracks.Add(new TrackChoice { Track = nextTrack, Votes = 0 });
            tracksLeft.Remove(nextTrack);

            _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $" /votetrack {i} - {nextTrack.Name}" });
        }

        _votingOpen = true;
        await Task.Delay(_configuration.VotingDurationMilliseconds, stoppingToken);
        _votingOpen = false;

        int maxVotes = _availableTracks.Max(w => w.Votes);
        List<TrackType?> tracks = _availableTracks.Where(w => w.Votes == maxVotes).Select(w => w.Track).ToList();

        var winner = tracks[Random.Shared.Next(tracks.Count)];


        if (last.Type == winner)
        {
            _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $"Track vote ended. Staying on track for {_configuration.VotingIntervalMinutes} more minutes." });
        }
        else
        {
            _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $"Track vote ended. Next track: {winner.Name}" });
            _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $"Track will change in {_configuration.TransitionDurationMinutes} minutes." });

            // Delay the track switch by configured time delay
            await Task.Delay(_configuration.TransitionDurationMinutes, stoppingToken);

            _trackManager.SetTrack(new TrackData(last.Type, winner)
            {
                TransitionDuration = _configuration.TransitionDurationMilliseconds,
                UpdateContentManager = _configuration.UpdateContentManager
            });
        }
    }

    private async Task ExecuteAdminAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_adminTrackChange)
                {
                    _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $"Next track: {_adminTrack.UpcomingType.Name}" });
                    _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = $"Track will change in {_configuration.TransitionDurationMinutes} minutes." });

                    // Delay the track switch by configured time delay
                    await Task.Delay(_configuration.TransitionDurationMinutes, stoppingToken);

                    if (_adminTrack != null)
                    {
                        _adminTrackChange = false;
                        _trackManager.SetTrack(_adminTrack);
                        _adminTrack = null;
                    }
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during voting track update");
            }
            finally
            {
                await Task.Delay(60_000, stoppingToken);
            }
        }
    }
}
