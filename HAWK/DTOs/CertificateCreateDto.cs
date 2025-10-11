using System.ComponentModel.DataAnnotations;

namespace HAWK.DTOs
{
    public class CertificateCreateDto
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public IFormFile image { get; set; }
    }
}
