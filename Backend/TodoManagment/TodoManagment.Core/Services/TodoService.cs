using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TodoManagment.Core.Domain.Entities;
using TodoManagment.Core.Domain.Events;
using TodoManagment.Core.Domain.RepositoryContract;
using TodoManagment.Core.Dtos;
using TodoManagment.Core.Dtos.TodoDto;
using TodoManagment.Core.Exceptions;
using TodoManagment.Core.Helper;
using TodoManagment.Core.ServiceContract;

namespace TodoManagment.Core.Services
{
    public class TodoService(IUnitOfWork _unitOfWork, ILogger<TodoService> _logger)
        : ITodoService
    {
        public async Task<TodoResponse> CreateTodoAsync(TodoAddRequest todoAddRequest)
        {
            _logger.LogInformation("Creating a new Todo item with title: {Title}", todoAddRequest.Title);
            if (todoAddRequest is null)
            {
                _logger.LogError("TodoAddRequest is null");
                throw new ArgumentNullException(nameof(todoAddRequest));
            }

            var todo = todoAddRequest.Adapt<Todo>();
            _logger.LogInformation("Todo item created with title: {Title}", todo.Title);

            await _unitOfWork.Repository<Todo>().CreateAsync(todo);
            _logger.LogInformation("Todo item saved to the database with title: {Title}", todo.Title);
            return todo.Adapt<TodoResponse>();
        }

        public async Task<bool> DeleteTodoAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete Todo item with ID: {Id}", id);
            var todo = await _unitOfWork.Repository<Todo>().GetByAsync(x => x.Id == id);

            _logger.LogInformation("Deleting Todo item with ID: {Id}", id);
            if (todo == null)
            {
                _logger.LogError("Todo item with ID: {Id} not found", id);
                throw new NotFoundException("Todo Not Found");
            }
            _logger.LogInformation("Todo item with ID: {Id} found, proceeding to delete", id);

            var result = await _unitOfWork.Repository<Todo>().DeleteAsync(todo);

            if (!result)
            {
                _logger.LogError("Failed to delete Todo item with ID: {Id}", id);
                return false;
            }
            return result;
        }

        public async Task<PaginatedResponse<TodoResponse>> GetAllTodosAsync(Expression<Func<Todo, bool>>? predicate = null, PaginationDto? pagination = null)
        {
            pagination ??= new PaginationDto();
            _logger.LogInformation("Fetching all products with pagination: PageIndex={PageIndex}, PageSize={PageSize}",
                pagination.PageIndex, pagination.PageSize);

            var todos = await _unitOfWork.Repository<Todo>()
                .GetAllAsync(
                    predicate,
                    sortBy: pagination.SortBy,
                    sortDirection: pagination.SortDirection,
                    pageSize: pagination.PageSize,
                    pageIndex: pagination.PageIndex);


            
            long todoCount = await _unitOfWork.Repository<Todo>().CountAsync(predicate);
            _logger.LogInformation("Total products found: {TotalCount}", todoCount);

            if (!todos.Any())
            {
                _logger.LogInformation("No products found matching the criteria.");
                return new PaginatedResponse<TodoResponse>
                {
                    PageIndex = pagination.PageIndex,
                    PageSize = pagination.PageSize,
                    Items = new List<TodoResponse>(),
                    TotalCount = 0
                };
            }
            _logger.LogInformation("Products found: {Products}", todos);
            return new PaginatedResponse<TodoResponse>
            {
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize,
                Items = todos.Adapt<IEnumerable<TodoResponse>>() ,
                TotalCount = todoCount
            };
        }

        public async Task<TodoResponse> GetTodoByIdAsync(Guid id)
        {
            var todo = await _unitOfWork.Repository<Todo>().GetByAsync(x => x.Id == id);
            _logger.LogInformation("Fetching Todo item with ID: {Id}", id);
            if (todo == null)
            {
                _logger.LogError("Todo item with ID: {Id} not found", id);
                throw new NotFoundException("Todo Not Found");
            }
            _logger.LogInformation("Todo item with ID: {Id} found", id);
            return todo.Adapt<TodoResponse>();
        }

        public async Task<bool> MarkAsCompletedAsync(Guid id)
        {
            var todo = await _unitOfWork.Repository<Todo>().GetByAsync(x => x.Id == id);

            if (todo == null)
            {
                _logger.LogError("Todo item with ID: {Id} not found", id);
                throw new NotFoundException("Todo Not Found");
            }

            if (todo.Status == TodoStatus.Completed)
            {
                _logger.LogWarning("Todo item with ID: {Id} is already completed", id);
                return true;
            }

            todo.Status = TodoStatus.Completed;
            todo.LastModifiedDate = DateTime.UtcNow;

            todo.AddDomainEvent(new TodoCompletedEvent(todo));

            await _unitOfWork.Repository<Todo>().UpdateAsync(todo);

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<TodoResponse> UpdateTodoAsync(TodoUpdateRequest todoUpdateRequest)
        {
            if (todoUpdateRequest is null)
            {
                throw new ArgumentNullException(nameof(todoUpdateRequest));
            }
            _logger.LogInformation("Updating Todo item with ID: {Id}", todoUpdateRequest.Id);
            var todo = await _unitOfWork.Repository<Todo>()
                .GetByAsync(x => x.Id == todoUpdateRequest.Id);

            if(todo == null)
            {
                _logger.LogError("Todo item with ID: {Id} not found", todoUpdateRequest.Id);
                throw new NotFoundException("Todo Not Found");
            }
            _logger.LogInformation("Todo item with ID: {Id} found, proceeding to update", todoUpdateRequest.Id);
            todoUpdateRequest.Adapt(todo);

            todo.LastModifiedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Todo>().UpdateAsync(todo);
            _logger.LogInformation("Todo item with ID: {Id} updated successfully", todoUpdateRequest.Id);
            await _unitOfWork.CompleteAsync();

            return todo.Adapt<TodoResponse>();
        }
    }
}
