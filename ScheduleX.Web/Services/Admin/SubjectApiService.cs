using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;

namespace ScheduleX.Web.Services.Admin
{
    public class SubjectApiService
    {
        private readonly ISubjectRepository _repo;

        public SubjectApiService(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Department>> GetDepartments()
        {
            try { return await _repo.GetDepartmentsAsync(); }
            catch { return new(); }
        }

        public async Task<List<Course>> GetCourses(int deptId)
        {
            try { return await _repo.GetCoursesByDepartmentAsync(deptId); }
            catch { return new(); }
        }

        public async Task<(bool, string)> Create(Subject model)
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
        public async Task<(bool success, string message)> Activate(int id)
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
        public async Task<(bool success, string message)> Update(Subject model)
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
        public async Task<(bool success, string message)> Delete(int id)
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
        public async Task<(bool, string)> BulkUpload(List<Subject> list, int courseId)
        {
            try
            {
                return await _repo.BulkInsertAsync(list, courseId);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<List<Subject>> GetAllSubjects()
        {
            try
            {
                return await _repo.GetAllSubjectsAsync();
            }
            catch
            {
                return new();
            }
        }
    }
}