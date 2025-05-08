using TodoManagment.Core.Helper;

namespace TodoManagment.Core.Dtos.TodoDto
{
    public class  TodoUpdateRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public TodoPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
