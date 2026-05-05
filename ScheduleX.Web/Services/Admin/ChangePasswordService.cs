using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using ScheduleX.Core.Entities;
using System.Security.Claims;

namespace ScheduleX.Web.Services.Admin
{
    public class ChangePasswordService
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthenticationStateProvider _authProvider;

        public ChangePasswordService(
            UserManager<User> userManager,
            AuthenticationStateProvider authProvider)
        {
            _userManager = userManager;
            _authProvider = authProvider;
        }

        public async Task<(bool, string)> ChangePassword(string currentPassword, string newPassword)
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var userPrincipal = authState.User;

            if (userPrincipal?.Identity?.IsAuthenticated != true)
                return (false, "User not authenticated");

            var user = await _userManager.GetUserAsync(userPrincipal);

            if (user == null)
                return (false, "User not found");

            // 🔥 IMPORTANT: this is enough (NO manual CheckPassword needed)
            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword
            );

            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return (true, "Password changed successfully");
        }
    }
}