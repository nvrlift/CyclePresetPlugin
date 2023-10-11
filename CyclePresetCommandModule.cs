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

    [Command("votetrack", "preset vote"), RequireConnectedPlayer]
    public void VoteTrack(int choice)
    {
        _cyclePreset.CountVote(Context.Client!, choice);
    }

    [Command("preset show"), RequireConnectedPlayer]
    public void GetCurrentTrack()
    {
        _cyclePreset.GetTrack(Context.Client!);
    }

    [Command("preset list"), RequireAdmin]
    public void AdminTrackList()
    {
        _cyclePreset.ListAllTracks(Context.Client!);
    }

    [Command("preset start vote"), RequireAdmin]
    public void AdminTrackVoteStart()
    {
        _cyclePreset.StartVote();
    }

    [Command("preset set"), RequireAdmin]
    public void AdminTrackSet(int choice)
    {
        _cyclePreset.SetTrack(Context.Client!, choice);
    }

    [Command("preset random"), RequireAdmin]
    public void AdminTrackSet()
    {
        _cyclePreset.RandomTrack(Context.Client!);
    }
}
