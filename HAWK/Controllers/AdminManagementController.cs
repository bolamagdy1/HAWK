using HAWK.DTOs;
using HAWK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AdminManagementController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public AdminManagementController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("create-admin")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateAdmin(CreateAdminDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = "Admin User"
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "Admin");

        return Ok("Admin created successfully");
    }
}
