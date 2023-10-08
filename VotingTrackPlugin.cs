using AssettoServer.Network.Tcp;
using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using AssettoServer.Server.Plugin;
using AssettoServer.Shared.Network.Packets.Shared;
using AssettoServer.Shared.Services;
using Microsoft.Extensions.Hosting;
using nvrlift.AssettoServer.Track;
using Serilog;

namespace VotingTrackPlugin;

public class VotingTrackPlugin : CriticalBackgroundService, IAssettoServerAutostart
{
    private readonly EntryCarManager _entryCarManager;
    private readonly TrackManager _trackManager;
    private readonly VotingTrackConfiguration _configuration;
    private readonly List<ACTcpClient> _alreadyVoted = new();
    private readonly List<TrackChoice> _availableTracks = new();
    private readonly List<VotingTrackType> _tracks;

    private bool _votingOpen = false;
    private bool _adminTrackChange = false;
    private TrackData? _adminTrack = null;

    private class TrackChoice
    {
        public VotingTrackType? Track { get; init; }
        public int Votes { get; set; }
    }

    public VotingTrackPlugin(VotingTrackConfiguration configuration, ACServerConfiguration acServerConfiguration,
        EntryCarManager entryCarManager, TrackManager trackManager,
        IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _configuration = configuration;
        _entryCarManager = entryCarManager;
        _trackManager = trackManager;

        _tracks = _configuration.VotingTrackTypes;

        VotingTrackType startType = new()
        {
            Name = _tracks.FirstOrDefault(t => t.TrackFolder == acServerConfiguration.Server.Track
                                               && t.TrackLayoutConfig == acServerConfiguration.Server.TrackConfig)?.Name
                   ?? acServerConfiguration.Server.Track.Split('/').Last(),
            TrackFolder = acServerConfiguration.Server.Track,
            TrackLayoutConfig = acServerConfiguration.Server.TrackConfig ?? "",
            CMLink = _tracks.FirstOrDefault(t => t.TrackFolder == acServerConfiguration.Server.Track
                                                 && t.TrackLayoutConfig == acServerConfiguration.Server.TrackConfig)
                ?.CMLink ?? "",
            CMVersion = _tracks.FirstOrDefault(t => t.TrackFolder == acServerConfiguration.Server.Track
                                                    && t.TrackLayoutConfig == acServerConfiguration.Server.TrackConfig)
                ?.CMVersion ?? ""
        };
        _trackManager.SetTrack(new TrackData(startType, null)
        {
            IsInit = true,
            ContentManager = _configuration.ContentManager,
            TransitionDuration = 0
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => ExecuteAdminAsync(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_configuration.VotingIntervalMilliseconds - _configuration.VotingDurationMilliseconds,
                stoppingToken);
            try
            {
                Log.Information($"Starting track vote.");
                await UpdateAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during voting track update");
            }
            // finally { }
        }
    }

    internal void ListAllTracks(ACTcpClient client)
    {
        client.SendPacket(new ChatMessage { SessionId = 255, Message = "List of all tracks:" });
        for (int i = 0; i < _tracks.Count; i++)
        {
            var track = _tracks[i];
            client.SendPacket(new ChatMessage { SessionId = 255, Message = $" /admintrackset {i} - {track.Name}" });
        }
    }

    internal void GetTrack(ACTcpClient client)
    {
        Log.Information(
            $"Current track: {_trackManager.CurrentTrack.Type!.Name} - {_trackManager.CurrentTrack.Type!.TrackFolder}");
        client.SendPacket(new ChatMessage
        {
            SessionId = 255,
            Message =
                $"Current track: {_trackManager.CurrentTrack.Type!.Name} - {_trackManager.CurrentTrack.Type!.TrackFolder}"
        });
    }

    internal void SetTrack(ACTcpClient client, int choice)
    {
        var last = _trackManager.CurrentTrack;

        if (choice < 0 && choice >= _tracks.Count)
        {
            Log.Information($"Invalid track choice.");
            client.SendPacket(new ChatMessage { SessionId = 255, Message = "Invalid track choice." });

            return;
        }

        var next = _tracks[choice];

        if (last.Type!.Equals(next))
        {
            Log.Information($"No change made, admin tried setting the current track.");
            client.SendPacket(new ChatMessage
                { SessionId = 255, Message = $"No change made, you tried setting the current track." });
        }
        else
        {
            _adminTrack = new TrackData(_trackManager.CurrentTrack.Type, next)
            {
                TransitionDuration = _configuration.TransitionDurationMilliseconds,
                ContentManager = _configuration.ContentManager
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

        client.SendPacket(new ChatMessage
            { SessionId = 255, Message = $"Your vote for {votedTrack.Track!.Name} has been counted." });
    }

    private async Task UpdateAsync(CancellationToken stoppingToken)
    {
        var last = _trackManager.CurrentTrack;

        _availableTracks.Clear();
        _alreadyVoted.Clear();

        var tracksLeft = new List<VotingTrackType>(_tracks);

        _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = "Vote for next track:" });
        for (int i = 0; i < _configuration.NumChoices; i++)
        {
            if (tracksLeft.Count < 1)
                break;
            var nextTrack = tracksLeft[Random.Shared.Next(tracksLeft.Count)];
            _availableTracks.Add(new TrackChoice { Track = nextTrack, Votes = 0 });
            tracksLeft.Remove(nextTrack);

            _entryCarManager.BroadcastPacket(new ChatMessage
                { SessionId = 255, Message = $" /votetrack {i} - {nextTrack.Name}" });
        }

        _votingOpen = true;
        await Task.Delay(_configuration.VotingDurationMilliseconds, stoppingToken);
        _votingOpen = false;

        int maxVotes = _availableTracks.Max(w => w.Votes);
        List<VotingTrackType?> tracks = _availableTracks.Where(w => w.Votes == maxVotes).Select(w => w.Track).ToList();

        var winner = tracks[Random.Shared.Next(tracks.Count)];


        if (last.Type!.Equals(winner!) || maxVotes == 0)
        {
            _entryCarManager.BroadcastPacket(new ChatMessage
            {
                SessionId = 255,
                Message = $"Track vote ended. Staying on track for {_configuration.VotingIntervalMinutes} more minutes."
            });
        }
        else
        {
            _entryCarManager.BroadcastPacket(new ChatMessage
                { SessionId = 255, Message = $"Track vote ended. Next track: {winner!.Name}" });
            _entryCarManager.BroadcastPacket(new ChatMessage
            {
                SessionId = 255, Message = $"Track will change in {_configuration.TransitionDurationMinutes} minutes."
            });

            // Delay the track switch by configured time delay
            await Task.Delay(_configuration.TransitionDurationMilliseconds, stoppingToken);

            _trackManager.SetTrack(new TrackData(last.Type, winner)
            {
                TransitionDuration = _configuration.TransitionDurationMilliseconds,
                ContentManager = _configuration.ContentManager
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
                    if (_adminTrack != null && !_adminTrack.Type!.Equals(_adminTrack.UpcomingType!))
                    {
                        Log.Information($"Next track: {_adminTrack!.UpcomingType!.Name}");
                        _entryCarManager.BroadcastPacket(new ChatMessage
                            { SessionId = 255, Message = $"Next track: {_adminTrack!.UpcomingType!.Name}" });
                        _entryCarManager.BroadcastPacket(new ChatMessage
                        {
                            SessionId = 255,
                            Message = $"Track will change in {_configuration.TransitionDurationMinutes} minutes."
                        });

                        // Delay the track switch by configured time delay
                        await Task.Delay(_configuration.TransitionDurationMilliseconds, stoppingToken);

                        _adminTrackChange = false;
                        _trackManager.SetTrack(_adminTrack);
                        _adminTrack = null;
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during voting track update");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
