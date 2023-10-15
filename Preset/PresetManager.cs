using AssettoServer.Shared.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CyclePresetPlugin.Preset;

public class PresetManager : CriticalBackgroundService
{
    private readonly PresetImplementation _presetImplementation;

    public PresetManager(PresetImplementation presetImplementation,
        IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _presetImplementation = presetImplementation;
    }
    private RestartType _restartType = RestartType.Disabled;

    public void SetRestartType(RestartType restartType)
    {
        _restartType = restartType;
    }

    public PresetData CurrentPreset { get; private set; } = null!;

    public void SetTrack(PresetData preset, RestartType restartType)
    {
        CurrentPreset = preset;

        if (!CurrentPreset.IsInit)
            UpdateTrack(restartType);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if(_restartType != RestartType.Disabled)
                    UpdateTrack(_restartType);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in track service update");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private void UpdateTrack(RestartType restartType)
    {
        if (CurrentPreset.UpcomingType != null && !CurrentPreset.Type!.Equals(CurrentPreset.UpcomingType!))
        {
            Log.Information($"Track change to '{CurrentPreset.UpcomingType!.Name}' initiated");
            _presetImplementation.ChangeTrack(CurrentPreset, restartType);

            CurrentPreset.Type = CurrentPreset.UpcomingType;
            CurrentPreset.UpcomingType = null;
        }
    }
}
