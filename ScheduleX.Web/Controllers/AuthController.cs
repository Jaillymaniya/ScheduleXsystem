using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ScheduleX.Core.Entities;

namespace ScheduleX.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager,
                              UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Authenticated");
        }

        [HttpGet("refresh")]
        public IActionResult Refresh()
        {
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username)
                       ?? await _userManager.FindByEmailAsync(model.Username);

            if (user == null || !user.IsActive)
                return Redirect("/login?error=invalid");

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                false,
                false
            );

            if (!result.Succeeded)
                return Redirect("/login?error=invalid");

            // 🔥 REDIRECT BASED ON ROLE
            if (user.Role == UserRole.Admin)
                return Redirect("/admin/overview");

            return Redirect("/tt/overview");
        }

       [HttpPost("logout")]
[IgnoreAntiforgeryToken]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();

    foreach (var cookie in Request.Cookies.Keys)
    {
        Response.Cookies.Delete(cookie);
    }

    return Redirect("/login"); // 🔥 IMPORTANT
}
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }



}