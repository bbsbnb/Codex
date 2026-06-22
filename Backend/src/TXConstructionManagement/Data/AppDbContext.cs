using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<PrimaryDocument> PrimaryDocuments => Set<PrimaryDocument>();
    public DbSet<BusinessFlow> BusinessFlows => Set<BusinessFlow>();
    public DbSet<ApprovalRecord> ApprovalRecords => Set<ApprovalRecord>();
    public DbSet<MonthlyPlan> MonthlyPlans => Set<MonthlyPlan>();
    public DbSet<InspectionRecord> InspectionRecords => Set<InspectionRecord>();
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
    public DbSet<SettlementDetail> SettlementDetails => Set<SettlementDetail>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WarningRecord> WarningRecords => Set<WarningRecord>();
    public DbSet<ContractTable> ContractTables => Set<ContractTable>();
    public DbSet<ReviewMeeting> ReviewMeetings => Set<ReviewMeeting>();
    public DbSet<SystemNotification> SystemNotifications => Set<SystemNotification>();
    public DbSet<WorkflowTemplate> WorkflowTemplates => Set<WorkflowTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User-Role relationship
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            entity.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId);
            entity.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId);
        });

        // Department self-reference
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.Parent).WithMany(d => d.Children).HasForeignKey(d => d.ParentId).IsRequired(false);
        });

        // ProjectMember
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(pm => pm.Id);
            entity.HasOne(pm => pm.Project).WithMany(p => p.Members).HasForeignKey(pm => pm.ProjectId);
            entity.HasOne(pm => pm.User).WithMany().HasForeignKey(pm => pm.UserId);
        });

        // PrimaryDocument
        modelBuilder.Entity<PrimaryDocument>(entity =>
        {
            entity.HasOne(pd => pd.Project).WithMany(p => p.Documents).HasForeignKey(pd => pd.ProjectId);
        });

        // BusinessFlow
        modelBuilder.Entity<BusinessFlow>(entity =>
        {
            entity.HasOne(bf => bf.Project).WithMany(p => p.Flows).HasForeignKey(bf => bf.ProjectId);
        });

        // ApprovalRecord
        modelBuilder.Entity<ApprovalRecord>(entity =>
        {
            entity.HasOne(ar => ar.Flow).WithMany(bf => bf.Approvals).HasForeignKey(ar => ar.FlowId);
            entity.HasOne(ar => ar.Approver).WithMany().HasForeignKey(ar => ar.ApproverId);
        });

        // MonthlyPlan
        modelBuilder.Entity<MonthlyPlan>(entity =>
        {
            entity.HasOne(mp => mp.Project).WithMany(p => p.Plans).HasForeignKey(mp => mp.ProjectId);
        });

        // InspectionRecord
        modelBuilder.Entity<InspectionRecord>(entity =>
        {
            entity.HasOne(ir => ir.Project).WithMany(p => p.Inspections).HasForeignKey(ir => ir.ProjectId);
        });

        // PaymentRecord
        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasOne(pr => pr.Project).WithMany(p => p.Payments).HasForeignKey(pr => pr.ProjectId);
        });

        modelBuilder.Entity<ReviewMeeting>(entity =>
            {
                entity.HasOne(rm => rm.Project).WithMany().HasForeignKey(rm => rm.ProjectId);
            });

            modelBuilder.Entity<ContractTable>(entity =>
            {
                entity.HasOne(ct => ct.Project).WithMany().HasForeignKey(ct => ct.ProjectId);
            });

            base.OnModelCreating(modelBuilder);
    }
}