using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Web.Models.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; } // e.g. "Pending", "Completed"
        public DateTime DueDate { get; set; }

        public IFormFile UploadFile { get; set; }
    }
}
