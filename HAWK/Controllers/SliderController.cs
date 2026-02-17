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
    public class SliderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sliders = await _context.Sliders
                .Include(s => s.Images)
                .ToListAsync();

            return Ok(sliders);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var slider = await _context.Sliders
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.id == id);

            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            return Ok(slider);
        }

        // ================= CREATE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SliderCreateDto dto)
        {
            if (dto.image == null || dto.image.Length == 0)
                return BadRequest("Image file is required");

            var rootPath = Directory.GetCurrentDirectory();

            var imageFolder = Path.Combine(rootPath, "server/sliders/images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            // ===== MAIN IMAGE =====
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
                var videoFolder = Path.Combine(rootPath, "server/sliders/videos");
                if (!Directory.Exists(videoFolder))
                    Directory.CreateDirectory(videoFolder);

                var videoName = Guid.NewGuid() + Path.GetExtension(dto.video.FileName);
                var videoPath = Path.Combine(videoFolder, videoName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await dto.video.CopyToAsync(stream);
                }

                videoDbPath = "/server/sliders/videos/" + videoName;
            }

            var slider = new Slider
            {
                heading = dto.heading,
                text = dto.text,
                SliderLocationID = dto.SliderLocationID,
                image = "/server/sliders/images/" + imageName,
                video = videoDbPath
            };

            _context.Sliders.Add(slider);
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

                    _context.SliderImages.Add(new SliderImage
                    {
                        image = "/server/sliders/images/" + fileName,
                        SliderId = slider.id
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Ok(slider);
        }

        // ================= UPDATE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] SliderCreateDto dto)
        {
            var slider = await _context.Sliders
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.id == id);

            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            var rootPath = Directory.GetCurrentDirectory();
            var imageFolder = Path.Combine(rootPath, "server/sliders/images");

            slider.heading = dto.heading;
            slider.text = dto.text;
            slider.SliderLocationID = dto.SliderLocationID;

            // ===== REPLACE MAIN IMAGE =====
            if (dto.image != null && dto.image.Length > 0)
            {
                if (!string.IsNullOrEmpty(slider.image))
                {
                    var oldImagePath = Path.Combine(rootPath, slider.image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                slider.image = "/server/sliders/images/" + imageName;
            }

            // ===== REPLACE VIDEO =====
            if (dto.video != null && dto.video.Length > 0)
            {
                if (!string.IsNullOrEmpty(slider.video))
                {
                    var oldVideoPath = Path.Combine(rootPath, slider.video.TrimStart('/'));
                    if (System.IO.File.Exists(oldVideoPath))
                        System.IO.File.Delete(oldVideoPath);
                }

                var videoFolder = Path.Combine(rootPath, "server/sliders/videos");
                if (!Directory.Exists(videoFolder))
                    Directory.CreateDirectory(videoFolder);

                var videoName = Guid.NewGuid() + Path.GetExtension(dto.video.FileName);
                var videoPath = Path.Combine(videoFolder, videoName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await dto.video.CopyToAsync(stream);
                }

                slider.video = "/server/sliders/videos/" + videoName;
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

                    _context.SliderImages.Add(new SliderImage
                    {
                        image = "/server/sliders/images/" + fileName,
                        SliderId = slider.id
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(slider);
        }

        // ================= DELETE =================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _context.Sliders
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.id == id);

            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            var rootPath = Directory.GetCurrentDirectory();

            // Delete main image
            if (!string.IsNullOrEmpty(slider.image))
            {
                var imagePath = Path.Combine(rootPath, slider.image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            // Delete video
            if (!string.IsNullOrEmpty(slider.video))
            {
                var videoPath = Path.Combine(rootPath, slider.video.TrimStart('/'));
                if (System.IO.File.Exists(videoPath))
                    System.IO.File.Delete(videoPath);
            }

            // Delete gallery images
            foreach (var img in slider.Images)
            {
                var imgPath = Path.Combine(rootPath, img.image.TrimStart('/'));
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            _context.SliderImages.RemoveRange(slider.Images);
            _context.Sliders.Remove(slider);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Slider deleted successfully" });
        }
    }
}
