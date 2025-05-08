using System.Linq.Expressions;
using TodoManagment.Core.Domain.Entities;
using TodoManagment.Core.Dtos;
using TodoManagment.Core.Dtos.TodoDto;
using TodoManagment.Core.Helper;

namespace TodoManagment.Core.ServiceContract
{
    public interface ITodoService
    {
        Task<TodoResponse> CreateTodoAsync(TodoAddRequest todoAddRequest);
        Task<TodoResponse> UpdateTodoAsync(TodoUpdateRequest todoUpdateRequest);
        Task<bool> DeleteTodoAsync(Guid id);
        Task<TodoResponse> GetTodoByIdAsync(Guid id);
        Task<PaginatedResponse<TodoResponse>> GetAllTodosAsync(
            Expression<Func<Todo, bool>>? predicate = null,
            PaginationDto? pagination = null);
        Task<bool> MarkAsCompletedAsync(Guid id);
    }
}
