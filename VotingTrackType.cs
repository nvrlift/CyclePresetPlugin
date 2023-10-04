using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackType : TrackBaseType
{
    public VotingTrackType(TrackEntry input)
    {
        Name = input.Name;
        TrackFolder = input.TrackFolder;
        TrackLayoutConfig = input.TrackLayoutConfig;
        CMLink = input.CMLink ?? "";
        CMVersion = input.CMVersion ?? "";
    }
}
