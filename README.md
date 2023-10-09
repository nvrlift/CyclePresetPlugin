# VotingTrackPlugin

Plugin to let players vote for a server track at a specified interval.

## Commands

/currenttrack

/votetrack <number> (server will ask users to vote for new map as per configured timeframe)
### Admin commands
/restartserver

/admintracklist

/admintrackset <number> (this one will be explained by track list)

## Configuration

Requires AS-Restarter.exe from [nvrlift.AssettoServer.HostExtension](https://github.com/nvrlift/nvrlift.AssettoServer.HostExtension)

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
- { Name: Gunsai, TrackFolder: csp/0/../pk_gunma_cycle_sports_center, TrackLayoutConfig: gcsc_full_attack, CMLink: https://mega.nz/...... , CMVersion: 1.5 }
- { Name: Shutoko, TrackFolder: csp/0/../shuto_revival_project_beta, TrackLayoutConfig: overload_layout, CMLink: https://mega.nz/...... , CMVersion: 1.5 }

```
