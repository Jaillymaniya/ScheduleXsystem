
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
   
    public interface ISubjectSemesterRepository
    {
        Task<List<SubjectSemester>> GetAllAsync(
            int academicYearId,
            int courseId);

        Task<List<Semester>> GetSemestersAsync(int courseId);

        Task<List<Subject>> GetSubjectsAsync(int courseId);

        Task<(bool, string)> AddAsync(SubjectSemester model);

        Task<(bool, string)> UpdateAsync(SubjectSemester model);

        Task<(bool, string)> SoftDeleteAsync(int id);

        Task<(bool, string)> ActivateAsync(int id);

        Task<(bool, string)> BulkInsertAsync(
            List<SubjectSemester> list);

        Task<List<Course>> GetCoursesForCoordinatorAsync(int userId);
    }
}