using Microsoft.AspNetCore.Identity;
using ScheduleX.Core.Entities;
using ScheduleX.Web.DTOs.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class ProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContext;

    public ProfileService(UserManager<User> userManager,
                          IHttpContextAccessor httpContext)
    {
        _userManager = userManager;
        _httpContext = httpContext;
    }


    public async Task<(bool Success, string Message, EditProfileDto Data)> GetProfileAsync()
    {
        try
        {
            var username = _httpContext.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return (false, "User not logged in", null);

            var user = await _userManager.Users
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
                return (false, "User not found", null);

            return (true, "Success", new EditProfileDto
            {
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,

                // 🔥 NEW
                DepartmentName = user.Department?.DepartmentName ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    //public async Task<(bool Success, string Message, EditProfileDto Data)> GetProfileAsync()
    //{
    //    var username = _httpContext.HttpContext?.User?.Identity?.Name;

    //    if (string.IsNullOrEmpty(username))
    //        return (false, "User not logged in", null);

    //    var user = await _userManager.FindByNameAsync(username);

    //    if (user == null)
    //        return (false, "User not found", null);

    //    return (true, "Success", new EditProfileDto
    //    {
    //        FullName = user.FullName,
    //        UserName = user.UserName,
    //        PhoneNumber = user.PhoneNumber,
    //        Email = user.Email
    //    });
    //}

    public async Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileDto model)
    {
        var username = _httpContext.HttpContext?.User?.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return (false, "User not logged in");

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return (false, "User not found");

        user.FullName = model.FullName;
        user.UserName = model.UserName;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, "Profile updated successfully");
    }

    public async Task<(bool Success, string Message)> UpdatePasswordAsync(ChangePasswordDto model)
    {
        try
        {
            var username = _httpContext.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return (false, "User not logged in");

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return (false, "User not found");

            // 🔥 check new password is not same as old
            var isSame = await _userManager.CheckPasswordAsync(user, model.NewPassword);
            if (isSame)
                return (false, "New password cannot be same as old password");

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, "Password updated successfully");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }


}