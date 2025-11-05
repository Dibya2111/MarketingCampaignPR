using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.Dtos
{
    public class RegisterDto
    {

        [Required(ErrorMessage = "Username (email) is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format. It must contain '@' and '.com'.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = null!;
    }
}
