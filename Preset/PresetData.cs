namespace CyclePresetPlugin.Preset;

public class PresetData
{
    public PresetType? Type { get; set; }
    public PresetType? UpcomingType { get; set; }
    public double TransitionDuration { get; set; }
    public bool IsInit { get; set; }

    public PresetData(PresetType? type, PresetType? upcomingType)
    {
        Type = type;
        UpcomingType = upcomingType;
    }
}
