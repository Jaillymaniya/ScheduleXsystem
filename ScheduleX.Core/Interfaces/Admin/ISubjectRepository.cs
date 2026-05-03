using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync(int courseId);

        Task<List<Course>> GetCoursesAsync();
        Task<List<Course>> GetCoursesForCoordinatorAsync(int coordinatorId);

        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Course>> GetCoursesByDepartmentAsync(int departmentId);

        Task<(bool, string)> AddAsync(Subject subject);
        Task<(bool, string)> UpdateAsync(Subject subject);
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<(bool, string)> SoftDeleteAsync(int id);
        Task<(bool, string)> ActivateAsync(int id);

        Task<bool> IsSubjectCodeExists(string code, int? subjectId);

        Task<(bool, string)> BulkInsertAsync(List<Subject> list, int courseId);
    }
}