using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("You are authenticated 🎉");
    }
}
