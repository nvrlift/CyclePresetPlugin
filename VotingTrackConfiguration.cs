using AssettoServer.Server.Configuration;
using JetBrains.Annotations;
using nvrlift.AssettoServer;
using YamlDotNet.Serialization;

namespace VotingTrackPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class VotingTrackConfiguration : NvrliftBaseConfiguration, IValidateConfiguration<VotingTrackConfigurationValidator>
{
    public int NumChoices { get; init; } = 3;
    public int VotingIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public int TransitionDurationMinutes { get; set; } = 5;
    public bool ChangeTrackWithoutVotes { get; set; } = false;
    public bool IncludeStayOnTrackVote { get; set; } = true;

    [YamlIgnore] public int VotingIntervalMilliseconds => VotingIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
    [YamlIgnore] public int TransitionDurationMilliseconds => TransitionDurationMinutes * 60_000;

}
