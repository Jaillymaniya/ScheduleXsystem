using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;

public class TTOverviewService
{
    private readonly AppDbContext _context;

    public TTOverviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OverviewDto> GetOverview(int courseId, int termId, int yearId)
    {
        var totalFaculty = await _context.Faculties
            .Where(x => x.IsActive)
            .CountAsync();

        var availableFaculty = await _context.Faculties
            .Where(x => x.IsActive)
            .CountAsync(x => x.FacultyAvailabilities.Any());

        var totalSubjects = await _context.Subjects
            .Where(x => x.IsActive && x.CourseId == courseId)
            .CountAsync();

        var totalRooms = await _context.Rooms
            .Where(x => x.IsActive)
            .CountAsync();

        return new OverviewDto
        {
            TotalFaculty = totalFaculty,
            AvailableFaculty = availableFaculty,
            TotalSubjects = totalSubjects,
            TotalRooms = totalRooms
        };
    }
}