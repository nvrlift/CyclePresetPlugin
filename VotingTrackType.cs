using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackType : ITrackBaseType
{
    public required string Name { get; set; }
    public required string PresetFolder { get; set; }

    public VotingTrackType(TrackEntry input)
    {
        Name = input.Name;
        PresetFolder = input.PresetFolder;
    }

    public VotingTrackType()
    {
    }
}
