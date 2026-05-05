using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Web.Services.Admin;

namespace ScheduleX.Web.Services.Admin
{
    public class TTCoordinatorService : ITTCoordinatorService
    {
        private readonly ITTCoordinatorRepository _repo;
        private readonly EmailService _emailService;

        public TTCoordinatorService(
            ITTCoordinatorRepository repo,
            EmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public Task<List<User>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<User?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        //public async Task<bool> CreateAsync(User user, string password)
        //{
        //    if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
        //        return false;

        //    var result = await _repo.CreateAsync(user, password);

        //    if (!result)
        //        return false;

        //    await _emailService.SendEmailAsync(
        //        user.Email,
        //        "TT Coordinator Account Created",
        //        $"Hello {user.FullName}, you are now a TT Coordinator."
        //    );

        //    return true;
        //}

        public async Task<(bool, string)> CreateAsync(User user, string password)
        {
            var (result, message) = await _repo.CreateAsync(user, password);

            if (!result)
                return (false, message);

            await _emailService.SendEmailAsync(
                user.Email!,
                "TT Coordinator Account Created",
                $"Hello {user.FullName}, you are now a TT Coordinator."
            );

            return (true, message);
        }

        public Task<bool> UpdateAsync(User user)
            => _repo.UpdateAsync(user);

        public Task<bool> SoftDeleteAsync(int id)
            => _repo.SoftDeleteAsync(id);


        public async Task<bool> ActivateAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                return false;

            var result = await _repo.ActivateAsync(id);

            if (!result)
                return false;

            await _emailService.SendEmailAsync(
                user.Email!,
                "Account Activated",
                $"Hello {user.FullName}, your TT Coordinator account has been activated again."
            );

            return true;
        }
    }
}