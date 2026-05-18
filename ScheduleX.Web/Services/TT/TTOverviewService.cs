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

    public async Task<OverviewDto> GetOverview(
     int courseId,
     int academicTermId,
     int academicYearId)
    {
        var totalFaculty = await _context.SubjectFaculties
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                x.Division.CourseId == courseId &&
                x.IsActive)
            .Select(x => x.FacultyId)
            .Distinct()
            .CountAsync();

        var availableFaculty = await _context.SubjectFaculties
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                x.Division.CourseId == courseId &&
                x.IsActive &&
                x.Faculty.IsActive)
            .Select(x => x.FacultyId)
            .Distinct()
            .CountAsync();

        var totalSubjects = await _context.SubjectSemesters
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                x.Subject.CourseId == courseId &&
                x.IsActive)
            .CountAsync();

        var totalRooms = await _context.Rooms
            .Where(x => x.IsActive)
            .CountAsync();

        var totalDivisions = await _context.Divisions
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                x.CourseId == courseId &&
                x.IsActive)
            .CountAsync();

        var totalTemplates = await _context.TimeTableTemplates
            .Where(x => x.IsActive)
            .CountAsync();

        var generatedCount = await _context.TimeTableBatches
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                x.AcademicTermId == academicTermId &&
                x.CourseId == courseId)
            .CountAsync();

        var hasScheduleConfig = await _context.ScheduleConfigs
            .AnyAsync(x =>
                x.AcademicYearId == academicYearId &&
                x.CourseId == courseId &&
                x.IsActive);

        return new OverviewDto
        {
            TotalFaculty = totalFaculty,
            AvailableFaculty = availableFaculty,
            TotalSubjects = totalSubjects,
            TotalRooms = totalRooms,
            TotalDivisions = totalDivisions,
            TotalTemplates = totalTemplates,
            GeneratedCount = generatedCount,
            HasScheduleConfig = hasScheduleConfig
        };
    }
}