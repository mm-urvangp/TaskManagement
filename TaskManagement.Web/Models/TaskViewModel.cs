namespace TaskManagement.Web.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}
