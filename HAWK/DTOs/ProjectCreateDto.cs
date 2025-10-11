using System.ComponentModel.DataAnnotations;

namespace HAWK.DTOs
{
    public class ProjectCreateDto
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string area { get; set; }
        [Required]
        public string scope { get; set; }
        [Required]
        public string contractor { get; set; }
        [Required]
        public IFormFile image { get; set; }
    }
}
