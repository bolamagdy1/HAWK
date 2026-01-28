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

            var rootPath = Directory.GetCurrentDirectory();

            // ================= IMAGE =================
            var imageFolder = Path.Combine(rootPath, "server/projects/images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
            var imagePath = Path.Combine(imageFolder, imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await dto.image.CopyToAsync(stream);
            }

            string? videoDbPath = "";

            // ================= VIDEO (OPTIONAL) =================
            if (dto.video != null && dto.video.Length > 0)
            {
                var videoFolder = Path.Combine(rootPath, "server/projects/videos");
                if (!Directory.Exists(videoFolder))
                    Directory.CreateDirectory(videoFolder);

                var videoName = Guid.NewGuid() + Path.GetExtension(dto.video.FileName);
                var videoPath = Path.Combine(videoFolder, videoName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await dto.video.CopyToAsync(stream);
                }

                videoDbPath = "/server/projects/videos/" + videoName;
            }

            var project = new Project
            {
                title = dto.title,
                area = dto.area,
                scope = dto.scope,
                contractor = dto.contractor,
                image = "/server/projects/images/" + imageName,
                video = videoDbPath
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

            var rootPath = Directory.GetCurrentDirectory();

            // ================= IMAGE (REPLACE IF UPLOADED, REQUIRED TO EXIST) =================
            if (dto.image != null && dto.image.Length > 0)
            {
                // delete old image
                if (!string.IsNullOrEmpty(project.image))
                {
                    var oldImagePath = Path.Combine(rootPath, project.image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                var imageFolder = Path.Combine(rootPath, "server/projects/images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                project.image = "/server/projects/images/" + imageName;
            }
            else if (string.IsNullOrEmpty(project.image))
            {
                // Image must exist
                return BadRequest("Image is required.");
            }

            // ================= VIDEO (REPLACE IF UPLOADED, ELSE SET NULL) =================
            if (dto.video != null && dto.video.Length > 0)
            {
                // delete old video
                if (!string.IsNullOrEmpty(project.video))
                {
                    var oldVideoPath = Path.Combine(rootPath, project.video.TrimStart('/'));
                    if (System.IO.File.Exists(oldVideoPath))
                        System.IO.File.Delete(oldVideoPath);
                }

                var videoFolder = Path.Combine(rootPath, "server/projects/videos");
                if (!Directory.Exists(videoFolder))
                    Directory.CreateDirectory(videoFolder);

                var videoName = Guid.NewGuid() + Path.GetExtension(dto.video.FileName);
                var videoPath = Path.Combine(videoFolder, videoName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await dto.video.CopyToAsync(stream);
                }

                project.video = "/server/projects/videos/" + videoName;
            }
            else
            {
                // If no video uploaded, set to null
                project.video = "";
            }

            // ================= UPDATE OTHER FIELDS =================
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
