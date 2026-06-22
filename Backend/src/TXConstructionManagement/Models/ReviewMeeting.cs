namespace TXConstructionManagement.Models;

public class ReviewMeeting
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string MeetingTitle { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; } = DateTime.Now;
    public string Participants { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Conclusions { get; set; } = string.Empty;
    public string ActionItems { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public ReviewMeetingStatus Status { get; set; } = ReviewMeetingStatus.Draft;
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Project? Project { get; set; }
}

public enum ReviewMeetingStatus
{
    Draft = 0,
    Completed = 1,
    Archived = 2
}