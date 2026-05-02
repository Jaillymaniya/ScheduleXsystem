using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task ToggleStatusAsync(int id);
    }
}