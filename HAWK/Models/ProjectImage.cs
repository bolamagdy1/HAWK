using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HAWK.Models
{
    public class ProjectImage
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string image { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
    }
}
