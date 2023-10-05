using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackType : ITrackBaseType
{
    public string Name { get; set; }
    public string TrackFolder { get; set; }
    public string TrackLayoutConfig { get; set; }
    public string CMLink { get; set; }
    public string CMVersion { get; set; }
    public VotingTrackType(TrackEntry input, string name, string trackFolder, string trackLayoutConfig, string cmLink, string cmVersion)
    {
        Name = input.Name;
        TrackFolder = input.TrackFolder;
        TrackLayoutConfig = input.TrackLayoutConfig;
        CMLink = input.CMLink ?? "";
        CMVersion = input.CMVersion ?? "";
    }
    public VotingTrackType(){}
}
