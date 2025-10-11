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
    }
}
