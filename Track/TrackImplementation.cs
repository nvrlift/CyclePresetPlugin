using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using IniParser;
using IniParser.Model;
using Serilog;
using VotingTrackPlugin.ContentManager;

namespace VotingTrackPlugin.Track;

public class TrackImplementation
{
    private readonly ContentManagerImplementation _contentManagerImplementation;
    private readonly ACServerConfiguration _acServerConfiguration;
    private readonly ACServer _server;

    public TrackImplementation(ContentManagerImplementation contentManagerImplementation,
        ACServerConfiguration acServerConfiguration,
        ACServer server)
    {
        _contentManagerImplementation = contentManagerImplementation;
        _acServerConfiguration = acServerConfiguration;
        _server = server;
    }

    public void ChangeTrack(TrackData track)
    {
        
        var iniPath = Path.Join(_acServerConfiguration.BaseFolder, "server_cfg.ini");
        if (File.Exists(iniPath))
        {
            Log.Error("'server_cfg.ini' not found, track change starting...");

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniPath);
            
            // I am replicating ACServerConfiguration.Server
            // [IniField("SERVER", "TRACK")] public string Track { get; init; } = "";
            // [IniField("SERVER", "CONFIG_TRACK")] public string TrackConfig { get; init; } = "";
            data["SERVER"]["TRACK"] = track.UpcomingType.TrackFolder;
            data["SERVER"]["CONFIG_TRACK"] = track.UpcomingType.TrackLayoutConfig;
            
            parser.WriteFile(iniPath, data);
        }
        else
        {
            Log.Error("Couldn't change track, 'server_cfg.ini' not found.");
            return;
        }

        // Content Manager Changes
        if (track.UpdateContentManager)
            if(_contentManagerImplementation.UpdateTrackConfig(track))
                Log.Information("ContentManager configuration updated.");
            else
                Log.Error("Failed to update ContentManager configuration.");

        // Restart Server
        var restartFile = Path.Join(_acServerConfiguration.BaseFolder, $"{Environment.ProcessId}.asrestart");
        File.Create(restartFile);
    }
}
