using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs
{
    public class UserCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit and be 6+ characters.")]
        public string Password { get; set; }

        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be 10 digits.")]
        public string Mobile { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public IFormFile? ProfilePic { get; set; }

        public string Role { get; set; }
    }
}
