using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class ReviewMeetingService
{
    private readonly AppDbContext _context;
    public ReviewMeetingService(AppDbContext context) { _context = context; }

    public async Task<ReviewMeetingResponse> CreateAsync(CreateReviewMeetingRequest req, int userId)
    {
        var mt = new ReviewMeeting
        {
            ProjectId = req.ProjectId, MeetingTitle = req.MeetingTitle,
            Participants = req.Participants, Content = req.Content,
            Conclusions = req.Conclusions, ActionItems = req.ActionItems,
            ResponsiblePerson = req.ResponsiblePerson, Deadline = req.Deadline,
            CreatorId = userId, Status = ReviewMeetingStatus.Draft
        };
        _context.ReviewMeetings.Add(mt);
        await _context.SaveChangesAsync();
        return MapToResponse(mt);
    }

    public async Task<bool> CompleteAsync(int id)
    {
        var mt = await _context.ReviewMeetings.FindAsync(id);
        if (mt == null) return false;
        mt.Status = ReviewMeetingStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var mt = await _context.ReviewMeetings.FindAsync(id);
        if (mt == null) return false;
        mt.Status = ReviewMeetingStatus.Archived;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReviewMeetingResponse>> GetByProjectAsync(int projectId)
    {
        var list = await _context.ReviewMeetings.Where(m => m.ProjectId == projectId)
            .OrderByDescending(m => m.MeetingDate).ToListAsync();
        return list.Select(MapToResponse).ToList();
    }

    public async Task<ReviewMeetingResponse?> GetByIdAsync(int id)
    {
        var mt = await _context.ReviewMeetings.FindAsync(id);
        return mt == null ? null : MapToResponse(mt);
    }

    private ReviewMeetingResponse MapToResponse(ReviewMeeting m) => new()
    {
        Id = m.Id, ProjectId = m.ProjectId, MeetingTitle = m.MeetingTitle,
        MeetingDate = m.MeetingDate, Participants = m.Participants,
        Content = m.Content, Conclusions = m.Conclusions,
        ActionItems = m.ActionItems, ResponsiblePerson = m.ResponsiblePerson,
        Deadline = m.Deadline, Status = m.Status,
        StatusName = m.Status switch { ReviewMeetingStatus.Draft => "草稿", ReviewMeetingStatus.Completed => "已完成", ReviewMeetingStatus.Archived => "已归档", _ => "" },
        CreatedAt = m.CreatedAt
    };
}