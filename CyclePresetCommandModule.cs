using AssettoServer.Commands;
using AssettoServer.Commands.Attributes;
using Qmmands;

namespace CyclePresetPlugin;

public class CyclePresetCommandModule : ACModuleBase
{
    private readonly CyclePresetPlugin _cyclePreset;

    public CyclePresetCommandModule(CyclePresetPlugin cyclePreset)
    {
        _cyclePreset = cyclePreset;
    }

    [Command("votetrack", "vt", "votepreset", "vp", "presetvote", "pv"), RequireConnectedPlayer]
    public void VoteTrack(int choice)
    {
        _cyclePreset.CountVote(Context.Client!, choice);
    }

    [Command("presetshow", "currentpreset", "currentrack"), RequireConnectedPlayer]
    public void GetCurrentTrack()
    {
        _cyclePreset.GetTrack(Context.Client!);
    }

    [Command("presetlist", "presetget", "presets"), RequireAdmin]
    public void AdminTrackList()
    {
        _cyclePreset.ListAllPresets(Context.Client!);
    }

    [Command("presetstartvote", "presetvotestart"), RequireAdmin]
    public void AdminTrackVoteStart()
    {
        _cyclePreset.StartVote();
    }

    [Command("presetset", "presetchange", "presetuse", "presetupdate"), RequireAdmin]
    public void AdminTrackSet(int choice)
    {
        _cyclePreset.SetPreset(Context.Client!, choice);
    }

    [Command("presetrandom"), RequireAdmin]
    public void AdminTrackSet()
    {
        _cyclePreset.RandomTrack(Context.Client!);
    }
}
