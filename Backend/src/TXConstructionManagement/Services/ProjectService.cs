using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context) { _context = context; }

    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, int operatorId)
    {
        var project = new Project
        {
            ProjectCode = request.ProjectCode,
            ProjectName = request.ProjectName,
            ContractAmount = request.ContractAmount,
            StartDate = request.StartDate,
            ExpectedEndDate = request.ExpectedEndDate,
            Address = request.Address,
            ClientName = request.ClientName,
            Description = request.Description,
            Status = ProjectStatus.Active
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return MapToResponse(project);
    }

    public async Task<bool> UpdateProjectAsync(int projectId, UpdateProjectRequest request, int operatorId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return false;

        if (request.ProjectName != null) project.ProjectName = request.ProjectName;
        if (request.ContractAmount.HasValue) project.ContractAmount = request.ContractAmount.Value;
        if (request.StartDate.HasValue) project.StartDate = request.StartDate.Value;
        if (request.ExpectedEndDate.HasValue) project.ExpectedEndDate = request.ExpectedEndDate;
        if (request.ActualEndDate.HasValue) project.ActualEndDate = request.ActualEndDate;
        if (request.Status.HasValue) project.Status = request.Status.Value;
        if (request.Address != null) project.Address = request.Address;
        if (request.ClientName != null) project.ClientName = request.ClientName;
        if (request.Description != null) project.Description = request.Description;
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int operatorId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return false;
        project.Status = ProjectStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(int projectId)
    {
        var project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
        return project == null ? null : MapToResponse(project);
    }

    public async Task<List<ProjectResponse>> GetAllProjectsAsync()
    {
        var projects = await _context.Projects.Include(p => p.Members).OrderByDescending(p => p.CreatedAt).ToListAsync();
        return projects.Select(MapToResponse).ToList();
    }

    public async Task<List<ProjectResponse>> GetProjectsByStatusAsync(ProjectStatus status)
    {
        var projects = await _context.Projects.Where(p => p.Status == status).Include(p => p.Members).ToListAsync();
        return projects.Select(MapToResponse).ToList();
    }

    public async Task<bool> AssignMemberAsync(int projectId, int userId, Position position, int operatorId)
    {
        var exists = await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        if (exists) return false;

        _context.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = projectId, UserId = userId, Position = position
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveMemberAsync(int projectId, int userId, int operatorId)
    {
        var pm = await _context.ProjectMembers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
        if (pm == null) return false;
        _context.ProjectMembers.Remove(pm);
        await _context.SaveChangesAsync();
        return true;
    }

    private ProjectResponse MapToResponse(Project p) => new()
    {
        Id = p.Id, ProjectCode = p.ProjectCode, ProjectName = p.ProjectName,
        ContractAmount = p.ContractAmount, StartDate = p.StartDate,
        ExpectedEndDate = p.ExpectedEndDate, ActualEndDate = p.ActualEndDate,
        Status = p.Status, Address = p.Address, ClientName = p.ClientName,
        Description = p.Description, CreatedAt = p.CreatedAt,
        MemberCount = p.Members?.Count ?? 0
    };
}