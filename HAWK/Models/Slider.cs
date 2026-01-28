using System.ComponentModel.DataAnnotations;

namespace HAWK.Models
{
    public class Slider
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string image { get; set; }
        [Required]
        public string heading { get; set; }
        [Required]
        public string text { get; set; }
        public string video { get; set; }
    }
}
