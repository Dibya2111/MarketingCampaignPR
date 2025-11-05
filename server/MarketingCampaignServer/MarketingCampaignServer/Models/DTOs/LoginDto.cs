using System.ComponentModel.DataAnnotations;
namespace MarketingCampaignServer.Models.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username (email) is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters.")]
        public string Password { get; set; } = null!;
    }
}
