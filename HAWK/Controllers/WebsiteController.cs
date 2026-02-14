using HAWK.dbcontext;
using HAWK.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HAWK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public WebsiteController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ======================
        // GET About
        // ======================
        [HttpGet("about")]
        public async Task<IActionResult> GetAbout()
        {
            var about = await _context.WebsiteContents
                .FirstOrDefaultAsync(x => x.Key == "about");

            return Ok(about?.Value);
        }

        // ======================
        // UPDATE About
        // ======================
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("about")]
        public async Task<IActionResult> UpdateAbout(UpdateAboutDto dto)
        {
            var about = await _context.WebsiteContents
                .FirstOrDefaultAsync(x => x.Key == "about");

            if (about == null)
            {
                about = new Models.WebsiteSettings
                {
                    Key = "about",
                    Value = dto.Content
                };

                _context.WebsiteContents.Add(about);
            }
            else
            {
                about.Value = dto.Content;
            }

            await _context.SaveChangesAsync();

            return Ok("About updated successfully");
        }

        // ======================
        // UPLOAD Logo
        // ======================
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("logo")]
        public async Task<IActionResult> UploadLogo([FromForm] UploadLogoDto dto)
        {
            if (dto.Logo == null || dto.Logo.Length == 0)
                return BadRequest("Invalid file");

            var rootPath = Directory.GetCurrentDirectory();

            var uploadsFolder = Path.Combine(rootPath, "server/logos");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Logo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Logo.CopyToAsync(stream);
            }

            var logo = await _context.WebsiteContents
                .FirstOrDefaultAsync(x => x.Key == "logo");

            if (logo == null)
            {
                logo = new Models.WebsiteSettings
                {
                    Key = "logo",
                    Value = "/server/logos/" + fileName
                };

                _context.WebsiteContents.Add(logo);
            }
            else
            {
                logo.Value = "/server/logos/" + fileName;
            }

            await _context.SaveChangesAsync();

            return Ok("Logo uploaded successfully");
        }

        // ======================
        // GET Logo
        // ======================
        [HttpGet("logo")]
        public async Task<IActionResult> GetLogo()
        {
            var logo = await _context.WebsiteContents
                .FirstOrDefaultAsync(x => x.Key == "logo");

            return Ok(logo?.Value);
        }
    }

}
