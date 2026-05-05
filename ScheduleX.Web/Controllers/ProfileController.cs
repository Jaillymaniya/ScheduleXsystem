using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ScheduleX.Web.DTOs.Account;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ProfileService _service;

    public ProfileController(ProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetProfileAsync();

        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] EditProfileDto model)
    {
        var result = await _service.UpdateProfileAsync(model);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        var result = await _service.UpdatePasswordAsync(model);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

}