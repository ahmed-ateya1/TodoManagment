using TodoManagment.Core.Helper;

public class PaginationDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";

    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }

    public TodoStatus? Status { get; set; }
    public TodoPriority? Priority { get; set; }
}
