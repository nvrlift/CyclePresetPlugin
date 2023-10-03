namespace VotingTrackPlugin.Track;

public class TrackData
{
    public VotingTrackPlugin.Track.TrackType? Type { get; set; }
    public VotingTrackPlugin.Track.TrackType? UpcomingType { get; set; }
    public double TransitionDuration { get; set; }
    public bool UpdateContentManager { get; set; }
    public TrackData(VotingTrackPlugin.Track.TrackType? type, VotingTrackPlugin.Track.TrackType? upcomingType)
    {
        Type = type;
        UpcomingType = upcomingType;
    }
}
