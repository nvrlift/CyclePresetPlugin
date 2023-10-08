using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackType : ITrackBaseType
{
    public required string Name { get; set; }
    public required string TrackFolder { get; set; }
    public required string TrackLayoutConfig { get; set; }
    public required string CMLink { get; set; }
    public required string CMVersion { get; set; }

    public VotingTrackType(TrackEntry input)
    {
        Name = input.Name;
        TrackFolder = input.TrackFolder;
        TrackLayoutConfig = input.TrackLayoutConfig ?? "";
        CMLink = input.CMLink ?? "";
        CMVersion = input.CMVersion ?? "";
    }

    public VotingTrackType()
    {
    }
}
