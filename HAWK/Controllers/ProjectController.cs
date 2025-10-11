using HAWK.dbcontext;
using HAWK.DTOs;
using HAWK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HAWK.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/project
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }
        // GET: api/project/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            return Ok(project);
        }
        // POST: api/project
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProjectCreateDto dto)
        {
            if (dto.image == null || dto.image.Length == 0)
                return BadRequest("Image file is required");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "server");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.image.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.image.CopyToAsync(stream);
            }

            var project = new Project
            {
                title = dto.title,
                area = dto.area,
                scope = dto.scope,
                contractor = dto.contractor,
                image = "/server/" + fileName
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = project.id }, project);
        }
        // PUT: api/project/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProjectCreateDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            // If new image is uploaded
            if (dto.image != null && dto.image.Length > 0)
            {
                // Delete old file
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), project.image.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);

                // Save new image
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "server");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.image.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                project.image = "/server/" + fileName;
            }

            // Update fields
            project.title = dto.title;
            project.area = dto.area;
            project.scope = dto.scope;
            project.contractor = dto.contractor;

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // DELETE: api/project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            // Delete image from server
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), project.image.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully" });
        }
    }
}
