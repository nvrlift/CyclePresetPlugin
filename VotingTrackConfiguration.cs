using AssettoServer.Server.Configuration;
using AssettoServer.Server.Weather;
using JetBrains.Annotations;
using nvrlift.AssettoServer.Track;
using YamlDotNet.Serialization;
using TrackType = VotingTrackPlugin.Track.TrackType;

namespace VotingTrackPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class VotingTrackConfiguration : IValidateConfiguration<VotingTrackConfigurationValidator>
{
    public List<TrackType> AvailableTracks { get; init; } = new();
    public int NumChoices { get; init; } = 3;
    public int VotingIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public int TransitionDurationMinutes { get; set; } = 5;
    public bool UpdateContentManager { get; init; } = false;

    [YamlIgnore] public int VotingIntervalMilliseconds => VotingIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
    [YamlIgnore] public int TransitionDurationMilliseconds => TransitionDurationMinutes * 60_000;
}
