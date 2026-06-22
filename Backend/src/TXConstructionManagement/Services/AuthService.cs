using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace TXConstructionManagement.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);
        if (user == null) return null;

        var hash = ComputeHash(request.Password);
        if (user.PasswordHash != hash) return null;

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.RoleName)
            .ToListAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Token = GenerateToken(user),
            Roles = roles,
            DepartmentIds = new List<int> { user.DepartmentId }
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username)) return false;

        var user = new User
        {
            Username = request.Username,
            PasswordHash = ComputeHash(request.Password),
            RealName = request.RealName,
            Phone = request.Phone,
            DepartmentId = request.DepartmentId,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token) => !string.IsNullOrEmpty(token);

    public async Task<User?> GetUserByIdAsync(int userId) => await _context.Users.FindAsync(userId);

    public async Task<List<User>> GetUsersByDepartmentAsync(int departmentId)
    {
        return await _context.Users.Where(u => u.DepartmentId == departmentId && u.IsActive).ToListAsync();
    }

    private string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private string GenerateToken(User user)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var raw = $"{user.Id}:{user.Username}:{timestamp}";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }
}