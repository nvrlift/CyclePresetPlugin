using System.Reflection;
using AssettoServer.Network.Tcp;
using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using AssettoServer.Server.Plugin;
using AssettoServer.Shared.Network.Packets.Shared;
using AssettoServer.Shared.Services;
using Microsoft.Extensions.Hosting;
using nvrlift.AssettoServer.Preset;
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
    private readonly List<PresetType> _tracks;

    private bool _votingOpen = false;
    private bool _adminTrackChange = false;
    private TrackData? _adminTrack = null;
    private  CancellationToken _manualVoteCancellationToken; 
    public CancellationTokenSource StartVoteCts = new CancellationTokenSource();

    private class TrackChoice
    {
        public PresetType? Track { get; init; }
        public int Votes { get; set; }
    }

    public VotingTrackPlugin(VotingTrackConfiguration configuration, PresetConfigurationManager presetConfigurationManager, 
        ACServerConfiguration acServerConfiguration,
        EntryCarManager entryCarManager, TrackManager trackManager,
        IHostApplicationLifetime applicationLifetime, CSPServerScriptProvider scriptProvider) : base(applicationLifetime)
    {
        _configuration = configuration;
        _entryCarManager = entryCarManager;
        _trackManager = trackManager;
        _trackManager.SetRestartType(_configuration.Restart);

        _tracks = presetConfigurationManager.VotingPresetTypes; // TODO fill from config files

        PresetConfiguration startConfiguration = new() // Read from file
        {
            Name = _tracks.FirstOrDefault(t => t.PresetFolder == acServerConfiguration.BaseFolder)?.Name
                   ?? acServerConfiguration.Server.Track.Split('/').Last(),
            PresetFolder = acServerConfiguration.BaseFolder,
        };
        _trackManager.SetTrack(new TrackData(presetConfigurationManager.CurrentConfiguration.ToPresetType(), null)
        {
            IsInit = true,
            TransitionDuration = 0
        }, configuration.Restart);
        
        // Include Client Reconnection Script
        using var streamReader = new StreamReader(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("VotingTrackPlugin.lua.reconnectclient.lua")!);
        scriptProvider.AddScript(streamReader.ReadToEnd(), "reconnectclient.lua");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => ExecuteAdminAsync(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _manualVoteCancellationToken = StartVoteCts.Token;
            
            using (CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(_manualVoteCancellationToken, stoppingToken))
                await Task.Delay(_configuration.VotingIntervalMilliseconds - _configuration.VotingDurationMilliseconds,
                    linkedCts.Token);
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
            finally
            {
                StartVoteCts = new CancellationTokenSource();
            }
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
            $"Current track: {_trackManager.CurrentTrack.Type!.Name} - {_trackManager.CurrentTrack.Type!.PresetFolder}");
        client.SendPacket(new ChatMessage
        {
            SessionId = 255,
            Message =
                $"Current track: {_trackManager.CurrentTrack.Type!.Name} - {_trackManager.CurrentTrack.Type!.PresetFolder}"
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

        if (choice >= _availableTracks.Count || choice < 0)
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

        var tracksLeft = new List<PresetType>(_tracks);

        // Add current track to "Stay on track"
        if (_configuration.IncludeStayOnTrackVote)
        {
            _availableTracks.Add(new TrackChoice
            {
                Track = new PresetType
                {
                    Name = "Stay on current track",
                    PresetFolder = last.Type!.PresetFolder
                }
            });
            tracksLeft.Remove(new PresetType
            {
                Name = last.Type.Name,
                PresetFolder = last.Type.PresetFolder
            });
        }
        
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
        List<PresetType?> tracks = _availableTracks.Where(w => w.Votes == maxVotes).Select(w => w.Track).ToList();

        var winner = tracks[Random.Shared.Next(tracks.Count)];


        if (last.Type!.Equals(winner!) || (maxVotes == 0 && !_configuration.ChangeTrackWithoutVotes))
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
            }, _configuration.Restart);
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
                        _trackManager.SetTrack(_adminTrack, _configuration.Restart);
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
