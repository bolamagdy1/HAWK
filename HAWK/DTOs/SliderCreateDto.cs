using System.ComponentModel.DataAnnotations;

namespace HAWK.DTOs
{
    public class SliderCreateDto
    {
        [Required]
        public string heading { get; set; }
        [Required]
        public string text { get; set; }
        [Required]
        public IFormFile image { get; set; }
        public IFormFile? video { get; set; }
        [Required]
        public int SliderLocationID { get; set; }

        // Multiple Images
        public List<IFormFile>? images { get; set; }
    }
}
