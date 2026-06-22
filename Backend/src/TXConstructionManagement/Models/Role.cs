namespace TXConstructionManagement.Models;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}