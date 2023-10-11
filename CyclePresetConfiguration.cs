using AssettoServer.Server.Configuration;
using JetBrains.Annotations;
using nvrlift.AssettoServer;
using YamlDotNet.Serialization;

namespace CyclePresetPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class CyclePresetConfiguration : NvrliftBaseConfiguration, IValidateConfiguration<CyclePresetConfigurationValidator>
{
    public bool VoteEnabled { get; init; } = true;
    public int VoteChoices { get; init; } = 3;
    public bool ChangeTrackWithoutVotes { get; init; } = false;
    public bool IncludeStayOnTrackVote { get; init; } = true;
    
    // Cycle Numbers :)
    public int CycleIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public int TransitionDurationMinutes { get; init; } = 5;

    [YamlIgnore] public int CycleIntervalMilliseconds => CycleIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
    [YamlIgnore] public int TransitionDurationMilliseconds => TransitionDurationMinutes * 60_000;

}
