using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;

namespace ScheduleX.Web.Services.Admin
{
    public class DepartmentApiService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentApiService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<(bool success, string message)> CreateAsync(Department dept)
        {
            try
            {
                await _repo.AddAsync(dept);
                return (true, "Department added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> UpdateAsync(Department dept)
        {
            try
            {
                await _repo.UpdateAsync(dept);
                return (true, "Department updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> ToggleAsync(int id)
        {
            try
            {
                await _repo.ToggleStatusAsync(id);
                return (true, "Status updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}