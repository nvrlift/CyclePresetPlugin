using AssettoServer.Server.Configuration;
using AssettoServer.Shared.Network.Http.Responses;
using Newtonsoft.Json;
using Serilog;
using TrackData = VotingTrackPlugin.Track.TrackData;

namespace VotingTrackPlugin.ContentManager;

public class ContentManagerImplementation
{
    private readonly ACServerConfiguration _acServerConfiguration;

    public ContentManagerImplementation(ACServerConfiguration acServerConfiguration)
    {
        _acServerConfiguration = acServerConfiguration;
    }

    public bool UpdateTrackConfig(TrackData track)
    {
        if (track.UpcomingType == null)
            return false;
        
        var newContentManagerConfig = _acServerConfiguration.ContentConfiguration;
        if (_acServerConfiguration.ContentConfiguration == null)
            Log.Error("ContentManager configuration not found.");
            
        _acServerConfiguration.ContentConfiguration.Track = new CMContentEntryVersionized()
        {
            Url = track.Type.CMLink,
            Version = track.Type.CMVersion
        };

        string cmContentPath = Path.Join(_acServerConfiguration.BaseFolder, "cm_content/content.json");
        if (File.Exists(cmContentPath))
        {
            Log.Error("ContentManager configuration file not found.");

            var output = JsonConvert.SerializeObject(newContentManagerConfig, Formatting.Indented);
            
            File.WriteAllText(cmContentPath, output);
        }

        return true;
    }
}
