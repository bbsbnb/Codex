namespace TXConstructionManagement.Models;

public class Department
{
    public int Id { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public Department? Parent { get; set; }
    public ICollection<Department>? Children { get; set; }
    public string HeadName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}