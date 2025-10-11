using System.ComponentModel.DataAnnotations;

namespace HAWK.Models
{
    public class Certificate
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string image { get; set; }
    }
}
