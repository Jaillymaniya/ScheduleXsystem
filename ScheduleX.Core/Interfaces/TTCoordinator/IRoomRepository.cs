using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllAsync();

        Task<List<Department>> GetDepartmentsAsync();

        Task<Room?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(
            string roomName,
            int departmentId,
            int? excludeId = null);

        Task AddAsync(Room room);

        Task UpdateAsync(Room room);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}