using TXConstructionManagement.DTOs;

namespace TXConstructionManagement.Services.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, int operatorId);
    Task<bool> UpdateProjectAsync(int projectId, UpdateProjectRequest request, int operatorId);
    Task<bool> DeleteProjectAsync(int projectId, int operatorId);
    Task<ProjectResponse?> GetProjectByIdAsync(int projectId);
    Task<List<ProjectResponse>> GetAllProjectsAsync();
    Task<List<ProjectResponse>> GetProjectsByStatusAsync(ProjectStatus status);
    Task<bool> AssignMemberAsync(int projectId, int userId, Position position, int operatorId);
    Task<bool> RemoveMemberAsync(int projectId, int userId, int operatorId);
}