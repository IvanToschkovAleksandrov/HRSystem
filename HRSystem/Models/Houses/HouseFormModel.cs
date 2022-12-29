using HRSystem.Data.Models;
using HRSystem.Services.Models;
using System.ComponentModel.DataAnnotations;
using static HRSystem.Data.DataConstants;

namespace HRSystem.Models.Houses
{
    public class HouseFormModel
    {
        [Required]
        [StringLength(HouseTitleMaxLength, MinimumLength = HouseTitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(HouseAddressMaxLength, MinimumLength = HouseAddressMinLength)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(HouseDescriptionMaxLength, MinimumLength = HouseDescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; } = null!;

        [Range(typeof(decimal), "0.00", "2000.00", ConvertValueInInvariantCulture = true, ErrorMessage = "PricePerMonth must be a positive number and less than {2} lv.")]
        [Display(Name = "Price Per Month")]
        public decimal PricePerMonth { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<HouseCategoryServiceModel> Categories { get; set; } = new List<HouseCategoryServiceModel>();
    }
}
