using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;


namespace ScheduleX.Infrastructure.Repositories.Admin
{
    public class TTCoordinatorRepository : ITTCoordinatorRepository
    {
        private readonly UserManager<User> _userManager;


        public TTCoordinatorRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // READ ALL (only active)
        //public async Task<List<User>> GetAllAsync()
        //{
        //    return await _userManager.Users
        //        .Where(x => x.Role == UserRole.TTCoordinator && x.IsActive)
        //        .ToListAsync();
        //}

        public async Task<List<User>> GetAllAsync()
        {
            return await _userManager.Users
                .Where(x => x.Role == UserRole.TTCoordinator)
                .Include(x => x.Department)   // also needed for department name
                .ToListAsync();
        }

        // READ BY ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        // CREATE
        //public async Task<bool> CreateAsync(User user, string password)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(password))
        //            return false;

        //        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
        //            return false;

        //        if (await _userManager.FindByEmailAsync(user.Email) != null)
        //            return false;

        //        if (await _userManager.FindByNameAsync(user.UserName) != null)
        //            return false;

        //        if (_userManager.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
        //            return false;

        //        user.Role = UserRole.TTCoordinator;
        //        user.IsActive = true;

        //        var result = await _userManager.CreateAsync(user, password);

        //        return result.Succeeded;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public async Task<(bool, string)> CreateAsync(User user, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.FullName) ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.UserName) ||
                    string.IsNullOrWhiteSpace(user.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(password) ||
                    user.DepartmentId == null)
                {
                    return (false, "All fields are required");
                }

                if (user.PhoneNumber.Length != 10)
                    return (false, "Phone must be 10 digits");

                if (await _userManager.FindByEmailAsync(user.Email) != null)
                    return (false, "Email already exists");

                if (await _userManager.FindByNameAsync(user.UserName) != null)
                    return (false, "Username already exists");

                if (_userManager.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
                    return (false, "Phone number already exists");

                // 🔥 ONE TT PER DEPARTMENT
                if (_userManager.Users.Any(x =>
                    x.DepartmentId == user.DepartmentId &&
                    x.Role == UserRole.TTCoordinator &&
                    x.IsActive))
                {
                    return (false, "This department already has a TT Coordinator");
                }

                user.Role = UserRole.TTCoordinator;
                user.IsActive = true;

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var error = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, error);
                }

                return (true, "TT Coordinator created successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // UPDATE
        public async Task<bool> UpdateAsync(User user)
        {
            var existing = await _userManager.FindByIdAsync(user.Id.ToString());

            if (existing == null)
                return false;

            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.PhoneNumber = user.PhoneNumber;
            existing.DepartmentId = user.DepartmentId;

            var result = await _userManager.UpdateAsync(existing);
            return result.Succeeded;
        }

        // SOFT DELETE
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return false;

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


        public async Task<bool> ActivateAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return false;

            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}