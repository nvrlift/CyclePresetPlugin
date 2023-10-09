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
# Will track change randomly if no vote has been counted
ChangeTrackWithoutVotes: true
# Whether the current track should be part of the next vote, so no map change occurs.
IncludeStayOnTrackVote: true
```

### Presets

Create a folder `presets` in the directory of `AssettoServer.exe`.

Create copies of the `cfg` folder within the `presets` folder.

Rename the copies of the `cfg` folder to something that represents the preset you are creating.
Something like `Shutoko_low_bhp` or `LA_Canyon_hypercars`... You get the Idea.

Within each of those folders you now have to change the `server_cfg.ini` to feature the correct `TRACK` and `TRACK_LAYOUT`.
You can also just use the `cfg` folder of newly created presets from ContentManager.
Add the following file to each `presets` folder and change the values accordingly: `preset_cfg.yml`
```yaml  
# The name of the Track; You will see this when voting
Name: Shutoko Cut Up
# Settings for Plugin features.
# Set Enabled to false, to exclude the Preset from Plugin Track lists
RandomTrack:
  Weight: 1.0
  Enabled: false;
VotingTrack:
  Enabled: true;
```
