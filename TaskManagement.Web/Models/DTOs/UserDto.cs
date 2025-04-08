using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Web.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile? ProfilePic { get; set; }
        public string Role { get; set; }
    }
}
