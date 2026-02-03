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
        // GET: api/project
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sliders = await _context.Sliders.ToListAsync();
            return Ok(sliders);
        }
        // GET: api/slider/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            return Ok(slider);
        }
        // POST: api/slider
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SliderCreateDto dto)
        {
            if (dto.image == null || dto.image.Length == 0)
                return BadRequest("Image file is required");

            var rootPath = Directory.GetCurrentDirectory();

            // ================= IMAGE =================
            var imageFolder = Path.Combine(rootPath, "server/sliders/images");
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

            // ================= SAVE =================
            var slider = new Slider
            {
                heading = dto.heading,
                text = dto.text,
                image = "/server/sliders/images/" + imageName,
                video = videoDbPath
            };

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = slider.id }, slider);
        }
        // PUT: api/slider/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] SliderCreateDto dto)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            slider.heading = dto.heading;
            slider.text = dto.text;

            var rootPath = Directory.GetCurrentDirectory();

            // ================= IMAGE (REPLACE IF UPLOADED) =================
            if (dto.image != null && dto.image.Length > 0)
            {
                // delete old image
                if (!string.IsNullOrEmpty(slider.image))
                {
                    var oldImagePath = Path.Combine(rootPath, slider.image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                var imageFolder = Path.Combine(rootPath, "server/sliders/images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var imageName = Guid.NewGuid() + Path.GetExtension(dto.image.FileName);
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                slider.image = "/server/sliders/images/" + imageName;
            }
            else if (string.IsNullOrEmpty(slider.image))
            {
                // Image must exist
                return BadRequest("Image is required.");
            }

            // ================= VIDEO (REPLACE OR NULL) =================
            if (dto.video != null && dto.video.Length > 0)
            {
                // delete old video if exists
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
            else
            {
                // If no video uploaded, set to null
                slider.video = "";
            }

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();

            return Ok(slider);
        }




        // DELETE: api/slider/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound($"Slider with ID {id} not found.");

            // delete image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), slider.image.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
