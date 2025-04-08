using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class TaskCreateRequest
    {
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
