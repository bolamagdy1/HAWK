using System.ComponentModel.DataAnnotations;

namespace HAWK.DTOs
{
    public class CareerApplicationDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public IFormFile CV { get; set; }
    }
}
