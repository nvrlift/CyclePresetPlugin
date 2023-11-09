using AssettoServer.Server.Configuration;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace CyclePresetPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class CyclePresetConfiguration : IValidateConfiguration<CyclePresetConfigurationValidator>
{
    // General settings
    public RestartType Restart { get; init; } = RestartType.Disabled;
    public bool ReconnectEnabled { get; init; } = true;
    
    // Voting settings
    public bool VoteEnabled { get; init; } = true; 
    public int VoteChoices { get; init; } = 3;
    public bool ChangeTrackWithoutVotes { get; init; } = false;
    public bool IncludeStayOnTrackVote { get; init; } = true;
    
    // Cycle numbers :)
    public int CycleIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public int TransitionDurationMinutes { get; init; } = 5;

    [YamlIgnore] public int CycleIntervalMilliseconds => CycleIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
    [YamlIgnore] public int TransitionDurationMilliseconds => TransitionDurationMinutes * 60_000;
    
}

public enum RestartType
{
    Disabled,
    WindowsFile,
    LinuxFile,
    File,
    Docker
}
