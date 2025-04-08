using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class TaskAssignmentModel
    {
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public TaskModel Task { get; set; }
        public UserModel User { get; set; }
    }
}
