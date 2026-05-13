using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Web.Services.TT;

public class TimeTableTemplateService
{
    private readonly AppDbContext _context;

    public TimeTableTemplateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeTableTemplate>> GetAllAsync()
    {
        return await _context.TimeTableTemplates
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.TemplateName)
            .ToListAsync();
    }

    public async Task<List<TimeTableTemplate>> GetActiveAsync()
    {
        return await _context.TimeTableTemplates
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.TemplateName)
            .ToListAsync();
    }

    public async Task<TimeTableTemplate?> GetByIdAsync(int id)
    {
        return await _context.TimeTableTemplates
            .FirstOrDefaultAsync(x => x.TemplateId == id);
    }

    public async Task<(bool Success, string Message)> CreateAsync(TimeTableTemplate template)
    {
        var exists = await _context.TimeTableTemplates
            .AnyAsync(x =>
                x.TemplateName.ToLower() ==
                template.TemplateName.ToLower());

        if (exists)
            return (false, "Template name already exists.");

        if (template.IsDefault)
        {
            var defaults = await _context.TimeTableTemplates
                .Where(x => x.IsDefault)
                .ToListAsync();

            foreach (var item in defaults)
                item.IsDefault = false;
        }

        template.CreatedAt = DateTime.Now;
        template.IsActive = true;

        _context.TimeTableTemplates.Add(template);

        await _context.SaveChangesAsync();

        return (true, "Template created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(TimeTableTemplate model)
    {
        var entity = await _context.TimeTableTemplates
            .FirstOrDefaultAsync(x => x.TemplateId == model.TemplateId);

        if (entity == null)
            return (false, "Template not found.");

        var duplicate = await _context.TimeTableTemplates
            .AnyAsync(x =>
                x.TemplateId != model.TemplateId &&
                x.TemplateName.ToLower() ==
                model.TemplateName.ToLower());

        if (duplicate)
            return (false, "Another template already exists.");

        entity.TemplateName = model.TemplateName;
        entity.LayoutType = model.LayoutType;
        entity.TemplateJson = model.TemplateJson;
        entity.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        return (true, "Template updated successfully.");
    }

    public async Task<(bool Success, string Message)> SetDefaultAsync(int id)
    {
        var template = await _context.TimeTableTemplates
            .FirstOrDefaultAsync(x => x.TemplateId == id);

        if (template == null)
            return (false, "Template not found.");

        var all = await _context.TimeTableTemplates.ToListAsync();

        foreach (var item in all)
            item.IsDefault = false;

        template.IsDefault = true;

        await _context.SaveChangesAsync();

        return (true, "Default template updated.");
    }

    public async Task<(bool Success, string Message)> SoftDeleteAsync(int id)
    {
        var entity = await _context.TimeTableTemplates
            .FirstOrDefaultAsync(x => x.TemplateId == id);

        if (entity == null)
            return (false, "Template not found.");

        entity.IsActive = false;
        entity.IsDefault = false;

        await _context.SaveChangesAsync();

        return (true, "Template deleted successfully.");
    }

    public async Task<(bool Success, string Message)> RestoreAsync(int id)
    {
        var entity = await _context.TimeTableTemplates
            .FirstOrDefaultAsync(x => x.TemplateId == id);

        if (entity == null)
            return (false, "Template not found.");

        entity.IsActive = true;

        await _context.SaveChangesAsync();

        return (true, "Template restored successfully.");
    }
}