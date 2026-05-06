using ScheduleX.Core.Entities;

using ScheduleX.Core.Interfaces.TTCoordinator;

namespace ScheduleX.Web.Services.TT
{
    public class SubjectSemesterApiService
    {
        private readonly ISubjectSemesterRepository _repo;

        public SubjectSemesterApiService(
            ISubjectSemesterRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SubjectSemester>> GetAll(
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

        public async Task<List<Semester>> GetSemesters(
            int courseId)
        {
            try
            {
                return await _repo.GetSemestersAsync(courseId);
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Subject>> GetSubjects(
            int courseId)
        {
            try
            {
                return await _repo.GetSubjectsAsync(courseId);
            }
            catch
            {
                return new();
            }
        }

        public async Task<(bool, string)> Create(
            SubjectSemester model)
        {
            try
            {
                return await _repo.AddAsync(model);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> Update(
            SubjectSemester model)
        {
            try
            {
                return await _repo.UpdateAsync(model);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> Delete(int id)
        {
            try
            {
                return await _repo.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> Activate(int id)
        {
            try
            {
                return await _repo.ActivateAsync(id);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> BulkUpload(
            List<SubjectSemester> list)
        {
            try
            {
                return await _repo.BulkInsertAsync(list);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}