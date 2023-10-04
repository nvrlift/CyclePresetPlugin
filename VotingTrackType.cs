using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackType : TrackBaseType
{
    public float Weight { get; set; } = 1.0f;
    public VotingTrackType(WeightEntry input)
    {
        Name = input.Name;
        TrackFolder = input.TrackFolder;
        TrackLayoutConfig = input.TrackLayoutConfig;
        CMLink = input.CMLink ?? "";
        CMVersion = input.CMVersion ?? "";
        Weight = input.Weight ?? 1.0f;
    }
}
