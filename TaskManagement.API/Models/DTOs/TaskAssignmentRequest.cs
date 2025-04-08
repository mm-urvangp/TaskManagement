using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs
{
    public class TaskAssignmentRequest
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
