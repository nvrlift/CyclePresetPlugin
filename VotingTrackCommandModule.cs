using AssettoServer.Commands;
using AssettoServer.Commands.Attributes;
using AssettoServer.Commands.Modules;
using Qmmands;

namespace VotingTrackPlugin;

public class VotingTrackCommandModule : ACModuleBase
{
    private readonly VotingTrack _votingTrack;

    public VotingTrackCommandModule(VotingTrack votingTrack)
    {
        _votingTrack = votingTrack;
    }

    [Command("votetrack"), RequireConnectedPlayer]
    public void VoteTrack(int choice)
    {
        _votingTrack.CountVote(Context.Client!, choice);
    }

    [Command("admintracklist"), RequireAdmin]
    public void AdminTrackList()
    {
        _votingTrack.ListAllTracks(Context.Client!);
    }

    [Command("admintrackset"), RequireAdmin]
    public void AdminTrackSet(int choice)
    {
        _votingTrack.SetTrack(Context.Client!, choice);
    }
}
