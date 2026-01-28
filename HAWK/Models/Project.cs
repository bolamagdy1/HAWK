using System.ComponentModel.DataAnnotations;

namespace HAWK.Models
{
    public class Project
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string image { get; set; }
        [Required]
        public string area { get; set; }
        [Required]
        public string scope { get; set; }
        [Required]
        public string contractor { get; set; }
        public string video { get; set; }
    }
}
