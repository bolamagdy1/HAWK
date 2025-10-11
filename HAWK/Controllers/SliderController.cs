using HAWK.dbcontext;
using HAWK.DTOs;
using HAWK.Models;
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
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SliderCreateDto dto)
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

            var slider = new Slider
            {
                heading = dto.heading,
                text = dto.text,
                image = "/server/" + fileName
            };

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = slider.id }, slider);
        }
        // PUT: api/slider/{id}
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

            if (dto.image != null && dto.image.Length > 0)
            {
                // delete old image if exists
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), slider.image.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                // save new image
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "server");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.image.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                slider.image = "/server/" + fileName;
            }

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();

            return Ok(slider);
        }

        // DELETE: api/slider/{id}
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
