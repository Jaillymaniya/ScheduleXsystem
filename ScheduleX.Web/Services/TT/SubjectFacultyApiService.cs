using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;

namespace ScheduleX.Web.Services.TT
{
    public class SubjectFacultyApiService
    {
        private readonly ISubjectFacultyRepository _repo;

        public SubjectFacultyApiService(
            ISubjectFacultyRepository repo)
        {
            _repo = repo;
        }

        // =====================================================
        // TABLE
        // =====================================================

        public async Task<List<SubjectFaculty>> GetAll(
            int academicYearId,
            int courseId)
        {
            try
            {
                return await _repo.GetAllAsync(
                    academicYearId,
                    courseId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // SEMESTERS
        // =====================================================

        public async Task<List<Semester>>
            GetSemesters(int courseId)
        {
            try
            {
                return await _repo
                    .GetSemestersAsync(courseId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // DIVISIONS
        // =====================================================

        public async Task<List<Division>> GetDivisions(
     int academicYearId,
     int courseId,
     int semesterId)
        {
            try
            {
                return await _repo
                    .GetDivisionsAsync(
                        academicYearId,
                        courseId,
                        semesterId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // SUBJECT SEMESTERS
        // =====================================================

        public async Task<List<SubjectSemester>>
            GetSubjectSemesters(
                int academicYearId,
                int semesterId)
        {
            try
            {
                return await _repo
                    .GetSubjectSemestersAsync(
                        academicYearId,
                        semesterId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // FACULTIES
        // =====================================================

        public async Task<List<Faculty>>
            GetFaculties(int courseId)
        {
            try
            {
                return await _repo
                    .GetFacultiesAsync(courseId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // EXTERNAL FACULTIES
        // =====================================================

        public async Task<List<Faculty>>
            GetExternalFaculties(
                int departmentId)
        {
            try
            {
                return await _repo
                    .GetExternalFacultiesAsync(
                        departmentId);
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // DEPARTMENTS
        // =====================================================

        public async Task<List<Department>>
            GetDepartments()
        {
            try
            {
                return await _repo
                    .GetDepartmentsAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // FACULTY BY EMAIL
        // =====================================================

        public async Task<Faculty?>
            GetFacultyByEmail(string email)
        {
            try
            {
                return await _repo
                    .GetFacultyByEmailAsync(email);
            }
            catch
            {
                return null;
            }
        }

        // =====================================================
        // FACULTY PERMISSION
        // =====================================================

        public async Task<bool>
            IsFacultyAllowed(
                int facultyId,
                int departmentId)
        {
            try
            {
                return await _repo
                    .IsFacultyAllowedAsync(
                        facultyId,
                        departmentId);
            }
            catch
            {
                return false;
            }
        }

        // =====================================================
        // CREATE
        // =====================================================

        public async Task<(bool, string)>
            Create(SubjectFaculty model)
        {
            try
            {
                return await _repo
                    .AddAsync(model);
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<(bool, string)>
            Update(SubjectFaculty model)
        {
            try
            {
                return await _repo
                    .UpdateAsync(model);
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }

        // =====================================================
        // TOGGLE
        // =====================================================

        public async Task<(bool, string)>
            Toggle(int id)
        {
            try
            {
                return await _repo
                    .ToggleAsync(id);
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }

        // =====================================================
        // BULK CSV
        // =====================================================

        public async Task<(bool, string)>
            BulkUpload(
                List<SubjectFaculty> list)
        {
            try
            {
                return await _repo
                    .BulkInsertAsync(list);
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }
    }
}