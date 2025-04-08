namespace TaskManagement.API.Models.DTOs
{
    public class TaskAssignmentDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
