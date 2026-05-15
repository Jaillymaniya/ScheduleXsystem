using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin;

public interface ISemesterRepository
{
    Task<List<Semester>> GetAllAsync();
    Task<List<Semester>> GetByCourseAsync(int courseId);
    Task AddAsync(Semester semester);
    Task UpdateAsync(Semester semester);
    Task ToggleStatusAsync(int id);
    Task<List<Course>> GetAllCoursesAsync();
    Task<Semester?> GetByIdAsync(int id);

    // NEW
    Task<List<AcademicTerm>> GetTermsByCourseAsync(int courseId);
}