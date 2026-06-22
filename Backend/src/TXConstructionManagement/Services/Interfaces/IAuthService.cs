using TXConstructionManagement.DTOs;

namespace TXConstructionManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetUserByIdAsync(int userId);
    Task<List<User>> GetUsersByDepartmentAsync(int departmentId);
}