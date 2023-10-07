# VotingTrackPlugin

Plugin to let players vote for a server weather at a specified interval.

## Configuration

Enable the plugin in `extra_cfg.yml`

```yaml
EnablePlugins:
- VotingTrackPlugin
```

Example configuration (add to bottom of `extra_cfg.yml`)

```yaml
---
!VotingTrackConfiguration
# Should content manager download links be updated
ContentManager: true
Restart: WindowsFile # yet to implement ->Docker
# Number of choices players can choose from at each voting interval
NumChoices: 3
# How long the vote stays open
# Minimum 30, Default 300
VotingDurationSeconds: 300
# How often a vote takes place
# Minimum 5, Default 90
VotingIntervalMinutes: 90
# How often a vote takes place
# Minimum 1, Default 5
TransitionDurationMinutes: 10
# Tracks that can be voted on
# CM field only required with UpdateContentManager: true
AvailableTracks:
- { Name: Gunsai, TrackFolder: some/path/to/gunsai, TrackLayoutConfig: GunsaiTogue, CMLink: https://mega.nz/...... , CMVersion: 1.5 }
- { Name: Shutoko, TrackFolder: some/path/to/Shutoko, TrackLayoutConfig: Default, CMLink: https://mega.nz/...... , CMVersion: 1.5 }

```
