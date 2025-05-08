using TodoManagment.Core.Helper;

namespace TodoManagment.Core.Dtos.TodoDto
{
    public class TodoAddRequest
    {
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public TodoPriority Priority { get; set; } = TodoPriority.Medium;
        public DateTime? DueDate { get; set; }

    }
}
