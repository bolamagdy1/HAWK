using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HAWK.Models
{
    public class SliderImage
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string image { get; set; }

        [ForeignKey("Slider")]
        public int SliderId { get; set; }
        [JsonIgnore]
        public Slider Slider { get; set; }
    }
}
