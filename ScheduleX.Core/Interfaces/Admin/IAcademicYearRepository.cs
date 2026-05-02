using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces
{
    public interface IAcademicYearRepository
    {
        Task<List<AcademicYear>> GetAllAsync();
        Task AddAsync(AcademicYear year);
        Task UpdateAsync(AcademicYear year);
        Task ToggleStatusAsync(int id);
    }
}