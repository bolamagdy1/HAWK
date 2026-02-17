using HAWK.dbcontext;
using HAWK.DTOs;
using HAWK.Models;
using Microsoft.AspNetCore.Authorization;
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

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _context.Projects
                .Include(p => p.Images)
                .ToListAsync();

            return Ok(projects);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.id == id);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            return Ok(project);
        }

        // ================= CREATE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProjectCreateDto dto)
        {
            if (dto.image == null || dto.image.Length == 0)
                return BadRequest("Image file is required");

            var rootPath = Directory.GetCurrentDirectory();

            // ===== MAIN IMAGE =====
            var imageFolder = Path.Combine(rootPath, "server/projects/images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
            var imagePath = Path.Combine(imageFolder, imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await dto.image.CopyToAsync(stream);
            }

            // ===== VIDEO (OPTIONAL) =====
            string? videoDbPath = "";

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

            // ===== GALLERY IMAGES =====
            if (dto.images != null && dto.images.Any())
            {
                foreach (var file in dto.images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(imageFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.ProjectImages.Add(new ProjectImage
                    {
                        image = "/server/projects/images/" + fileName,
                        ProjectId = project.id
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Ok(project);
        }

        // ================= UPDATE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProjectCreateDto dto)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.id == id);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            var rootPath = Directory.GetCurrentDirectory();
            var imageFolder = Path.Combine(rootPath, "server/projects/images");

            // ===== REPLACE MAIN IMAGE IF PROVIDED =====
            if (dto.image != null && dto.image.Length > 0)
            {
                if (!string.IsNullOrEmpty(project.image))
                {
                    var oldImagePath = Path.Combine(rootPath, project.image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                project.image = "/server/projects/images/" + imageName;
            }

            // ===== REPLACE VIDEO IF PROVIDED =====
            if (dto.video != null && dto.video.Length > 0)
            {
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

            // ===== ADD NEW GALLERY IMAGES =====
            if (dto.images != null && dto.images.Any())
            {
                foreach (var file in dto.images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(imageFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.ProjectImages.Add(new ProjectImage
                    {
                        image = "/server/projects/images/" + fileName,
                        ProjectId = project.id
                    });
                }
            }

            // ===== UPDATE FIELDS =====
            project.title = dto.title;
            project.area = dto.area;
            project.scope = dto.scope;
            project.contractor = dto.contractor;

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // ================= DELETE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.id == id);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            var rootPath = Directory.GetCurrentDirectory();

            // Delete main image
            if (!string.IsNullOrEmpty(project.image))
            {
                var imagePath = Path.Combine(rootPath, project.image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            // Delete video
            if (!string.IsNullOrEmpty(project.video))
            {
                var videoPath = Path.Combine(rootPath, project.video.TrimStart('/'));
                if (System.IO.File.Exists(videoPath))
                    System.IO.File.Delete(videoPath);
            }

            // Delete gallery images
            foreach (var img in project.Images)
            {
                var imgPath = Path.Combine(rootPath, img.image.TrimStart('/'));
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            _context.ProjectImages.RemoveRange(project.Images);
            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully" });
        }
    }
}
