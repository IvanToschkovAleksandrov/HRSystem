using System.ComponentModel.DataAnnotations;
using static HRSystem.Data.DataConstants;

namespace HRSystem.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; init; }

        [Required]
        [StringLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public IEnumerable<House> Houses { get; init; } = new List<House>();
    }
}
