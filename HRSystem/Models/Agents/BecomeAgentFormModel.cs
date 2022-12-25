using System.ComponentModel.DataAnnotations;
using static HRSystem.Data.DataConstants;

namespace HRSystem.Models.Agents
{
    public class BecomeAgentFormModel
    {
        [Required]
        [StringLength(AgentPhoneNumberMaxLength, MinimumLength = AgentPhoneNumberMinLength)]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; init; } = null!;
    }
}
