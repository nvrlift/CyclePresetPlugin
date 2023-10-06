using AssettoServer.Server.Configuration;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace VotingTrackPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class VotingTrackConfiguration : NvrliftBaseConfiguration, IValidateConfiguration<VotingTrackConfigurationValidator>
{
    public VotingTrackConfiguration()
    {
        Track = true;
    }
    public List<TrackEntry> AvailableTracks { get; init; } = new();
    public int NumChoices { get; init; } = 3;
    public int VotingIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public int TransitionDurationMinutes { get; set; } = 5;
    public bool UpdateContentManager { get; init; } = false;

    [YamlIgnore] public int VotingIntervalMilliseconds => VotingIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
    [YamlIgnore] public int TransitionDurationMilliseconds => TransitionDurationMinutes * 60_000;

    [YamlIgnore]
    public List<VotingTrackType> VotingTrackTypes => AvailableTracks.Select(t => new VotingTrackType
    {
        Name = t.Name,
        TrackFolder = t.TrackFolder,
        TrackLayoutConfig = t.TrackLayoutConfig,
        CMLink = t.CMLink ?? "",
        CMVersion = t.CMVersion ?? "",
    }).ToList();
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public struct TrackEntry
{
    public string Name { get; init; }
    public string TrackFolder { get; init; }
    public string TrackLayoutConfig { get; init; }
    public string? CMLink { get; init; }
    public string? CMVersion { get; init; }
}
