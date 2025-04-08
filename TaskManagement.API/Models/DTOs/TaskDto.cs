namespace TaskManagement.API.Models.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string? UploadFilePath { get; set; }

    }
}
