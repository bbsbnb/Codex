using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class WorkflowConfigService
{
    private readonly AppDbContext _context;
    public WorkflowConfigService(AppDbContext context) { _context = context; }

    public async Task<WorkflowTemplate> CreateAsync(CreateWorkflowTemplateRequest req)
    {
        var tpl = new WorkflowTemplate
        {
            TemplateName = req.TemplateName, FlowType = req.FlowType,
            NodesJson = req.NodesJson, Description = req.Description
        };
        _context.WorkflowTemplates.Add(tpl);
        await _context.SaveChangesAsync();
        return tpl;
    }

    public async Task<List<WorkflowTemplate>> GetAllAsync()
    {
        return await _context.WorkflowTemplates.OrderBy(t => t.FlowType).ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tpl = await _context.WorkflowTemplates.FindAsync(id);
        if (tpl == null) return false;
        _context.WorkflowTemplates.Remove(tpl);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool active)
    {
        var tpl = await _context.WorkflowTemplates.FindAsync(id);
        if (tpl == null) return false;
        tpl.IsActive = active;
        await _context.SaveChangesAsync();
        return true;
    }
}