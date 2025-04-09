using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Web.Models.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        [Required]
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime DueDate { get; set; }

        public IFormFile UploadFile { get; set; }
    }
}
