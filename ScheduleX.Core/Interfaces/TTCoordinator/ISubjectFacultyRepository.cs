using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface ISubjectFacultyRepository
    {
        // ================= DROPDOWNS =================

        Task<List<Semester>> GetSemestersAsync(
            int courseId);

        Task<List<Division>> GetDivisionsAsync(
     int academicYearId,
     int courseId,
     int semesterId);

        Task<List<SubjectSemester>> GetSubjectSemestersAsync(
            int academicYearId,
            int semesterId);

        Task<List<Faculty>> GetFacultiesAsync(
            int courseId);

        Task<List<Department>> GetDepartmentsAsync();

        Task<List<Faculty>> GetExternalFacultiesAsync(
            int departmentId);

        Task<Faculty?> GetFacultyByEmailAsync(
            string email);

        Task<bool> IsFacultyAllowedAsync(
            int facultyId,
            int departmentId);

        // ================= TABLE =================

        Task<List<SubjectFaculty>> GetAllAsync(
            int academicYearId,
            int courseId);

        // ================= CRUD =================

        Task<(bool, string)> AddAsync(
            SubjectFaculty model);

        Task<(bool, string)> UpdateAsync(
            SubjectFaculty model);

        Task<(bool, string)> ToggleAsync(
            int id);

        // ================= CSV =================

        Task<(bool, string)> BulkInsertAsync(
            List<SubjectFaculty> list);
    }
}