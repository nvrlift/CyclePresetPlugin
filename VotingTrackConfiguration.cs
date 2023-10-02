﻿using AssettoServer.Server.Configuration;
using AssettoServer.Server.Weather;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace VotingTrackPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class VotingTrackConfiguration : IValidateConfiguration<VotingTrackConfigurationValidator>
{
    public List<AvailableTrack> AvailableTracks { get; init; } = new();
    public int NumChoices { get; init; } = 3;
    public int VotingIntervalMinutes { get; init; } = 90;
    public int VotingDurationSeconds { get; init; } = 300;
    public bool UpdateContentManager { get; init; } = false;

    [YamlIgnore] public int VotingIntervalMilliseconds => VotingIntervalMinutes * 60_000;
    [YamlIgnore] public int VotingDurationMilliseconds => VotingDurationSeconds * 1000;
}

public class AvailableTrack
{
    public required string Name { get; init; }
    public required string TrackFolder { get; init; }
    public required string TrackLayoutConfig { get; init; }
    public string CMLink { get; init; } = "";
    public string CMVersion{ get; init; } = "";
}
