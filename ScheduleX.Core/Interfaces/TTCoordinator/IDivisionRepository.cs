


using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface IDivisionRepository
    {
        //Task<List<Division>> GetAllAsync(int ttCoordinatorId);
        Task<List<Division>> GetAllAsync(
         int ttCoordinatorId);
        Task<List<Semester>> GetSemestersAsync();

        Task<Division?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(
          int academicYearId,
          int courseId,
          int semesterId,
          string divisionName,
          int ttCoordinatorId,
          int? excludeId = null);
        Task AddAsync(Division division);

        Task UpdateAsync(Division division);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}