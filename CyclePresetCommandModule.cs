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
        _cyclePreset.CountVote(Client!, choice);
    }

    [Command("presetshow", "currentpreset", "currentrack"), RequireConnectedPlayer]
    public void GetCurrentTrack()
    {
        _cyclePreset.GetTrack(Client!);
    }

    [Command("presetlist", "presetget", "presets"), RequireAdmin]
    public void AdminTrackList()
    {
        _cyclePreset.ListAllPresets(Client!);
    }

    [Command("presetstartvote", "presetvotestart"), RequireAdmin]
    public void AdminTrackVoteStart()
    {
        _cyclePreset.StartVote();
    }

    [Command("presetset", "presetchange", "presetuse", "presetupdate"), RequireAdmin]
    public void AdminTrackSet(int choice)
    {
        _cyclePreset.SetPreset(Client!, choice);
    }

    [Command("presetrandom"), RequireAdmin]
    public void AdminTrackSet()
    {
        _cyclePreset.RandomTrack(Client!);
    }
}
