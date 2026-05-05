using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin
{
    public interface ITTCoordinatorRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        //Task<bool> CreateAsync(User user, string password);
        Task<(bool, string)> CreateAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ActivateAsync(int id);
    }
}