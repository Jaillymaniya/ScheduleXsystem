
//using ScheduleX.Core.Entities;

//namespace ScheduleX.Core.Interfaces.Admin
//{
//    public interface ITTCoordinatorService
//    {
//        Task<List<User>> GetAllAsync();
//        Task<User?> GetByIdAsync(int id);
//        //Task<bool> CreateAsync(User user, string password);

//        Task<(bool, string)> CreateAsync(User user, string password);
//        Task<bool> UpdateAsync(User user);
//        Task<bool> SoftDeleteAsync(int id);
//        Task<bool> ActivateAsync(int id);
//    }
//}



using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin
{
    public interface ITTCoordinatorService
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        //Task<(bool, string)> CreateAsync(
        //    User user,
        //    string password,
        //    List<int> courseIds);
        Task<(bool, string)> CreateAsync(User user, string password, List<int> courseIds);
        //Task<bool> UpdateAsync(User user);
        Task<bool> UpdateAsync(User user, List<int> courseIds);

        Task<bool> SoftDeleteAsync(int id);

        Task<bool> ActivateAsync(int id);
    }
}