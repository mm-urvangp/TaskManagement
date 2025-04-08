using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be 10 digits.")]
        public string Mobile { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? ProfilePicPath { get; set; }
        public string Role { get; set; }
    }
}
