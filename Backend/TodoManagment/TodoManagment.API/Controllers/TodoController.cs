using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using TodoManagment.Core.Domain.Entities;
using TodoManagment.Core.Dtos;
using TodoManagment.Core.Dtos.TodoDto;
using TodoManagment.Core.Helper;
using TodoManagment.Core.ServiceContract;

namespace TodoManagment.API.Controllers
{
    /// <summary>
    /// Controller for managing Todo items.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="TodoController"/> class.
        /// </summary>
        /// <param name="todoService"></param>
        /// <param name="logger"></param>
        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new Todo item to the system.
        /// </summary>
        /// <param name="request">The request containing Todo details to add, including Title, Description, Priority (enum <see cref="TodoPriority"/> with values Low, Medium, High), and optional DueDate.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the operation.</returns>
        /// <response code="201">Todo created successfully.</response>
        /// <response code="400">Todo creation failed.</response>
        [HttpPost("addTodo")]
        public async Task<ActionResult<ApiResponse>> AddTodo(TodoAddRequest request)
        {
            _logger.LogInformation("Creating a new Todo with Title: {Title}", request.Title);
            var response = await _todoService.CreateTodoAsync(request);

            if (response == null)
            {
                _logger.LogError("Failed to create Todo with Title: {Title}", request.Title);
                return BadRequest(new ApiResponse
                {
                    Message = "Todo creation failed",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            _logger.LogInformation("Todo with Title: {Title} created successfully", request.Title);
            return Ok(new ApiResponse
            {
                Message = "Todo created successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Result = response
            });
        }

        /// <summary>
        /// Updates an existing Todo item.
        /// </summary>
        /// <param name="request">The request containing updated Todo details, including Id, Title, Description, Status (enum <see cref="TodoStatus"/> with values e.g., NotStarted, InProgress, Completed), Priority (enum <see cref="TodoPriority"/> with values Low, Medium, High), and optional DueDate.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the operation.</returns>
        /// <response code="200">Todo updated successfully.</response>
        /// <response code="400">Todo update failed.</response>
        [HttpPut("updateTodo")]
        public async Task<ActionResult<ApiResponse>> UpdateTodo(TodoUpdateRequest request)
        {
            _logger.LogInformation("Updating Todo with ID: {Id}", request.Id);
            var response = await _todoService.UpdateTodoAsync(request);
            if (response == null)
            {
                _logger.LogError("Failed to update Todo with ID: {Id}", request.Id);
                return BadRequest(new ApiResponse
                {
                    Message = "Todo update failed",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            _logger.LogInformation("Todo with ID: {Id} updated successfully", request.Id);
            return Ok(new ApiResponse
            {
                Message = "Todo updated successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }

        /// <summary>
        /// Deletes a Todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to delete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the operation.</returns>
        /// <response code="200">Todo deleted successfully.</response>
        /// <response code="404">Todo not found.</response>
        [HttpDelete("deleteTodo/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteTodo(Guid id)
        {
            _logger.LogInformation("Deleting Todo with ID: {Id}", id);
            var isDeleted = await _todoService.DeleteTodoAsync(id);
            if (!isDeleted)
            {
                _logger.LogError("Failed to delete Todo with ID: {Id}", id);
                return NotFound(new ApiResponse
                {
                    Message = "Todo not found",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            _logger.LogInformation("Todo with ID: {Id} deleted successfully", id);
            return Ok(new ApiResponse
            {
                Message = "Todo deleted successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Retrieves a Todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to retrieve.</param>
        /// <returns>An <see cref="ApiResponse"/> containing the Todo item details, including Priority (enum <see cref="TodoPriority"/>) and Status (enum <see cref="TodoStatus"/>).</returns>
        /// <response code="200">Todo fetched successfully.</response>
        /// <response code="404">Todo not found.</response>
        [HttpGet("getTodo/{id}")]
        public async Task<ActionResult<ApiResponse>> GetTodoById(Guid id)
        {
            _logger.LogInformation("Fetching Todo with ID: {Id}", id);
            var response = await _todoService.GetTodoByIdAsync(id);
            if (response == null)
            {
                _logger.LogError("Todo with ID: {Id} not found", id);
                return NotFound(new ApiResponse
                {
                    Message = "Todo not found",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            _logger.LogInformation("Todo with ID: {Id} fetched successfully", id);
            return Ok(new ApiResponse
            {
                Message = "Todo fetched successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }

        /// <summary>
        /// Retrieves all Todo items with optional filtering and pagination.
        /// </summary>
        /// <param name="pagination">
        /// The pagination and filtering parameters:
        /// <list type="bullet">
        /// <item><description><b>PageNumber</b>: The page number for pagination.</description></item>
        /// <item><description><b>PageSize</b>: The number of items per page.</description></item>
        /// <item><description><b>Status</b>: Optional filter by Todo status (<see cref="TodoStatus"/>).</description></item>
        /// <item><description><b>Priority</b>: Optional filter by Todo priority (<see cref="TodoPriority"/>).</description></item>
        /// <item><description><b>DueDateFrom</b>: Optional filter to include Todos due on or after this date.</description></item>
        /// <item><description><b>DueDateTo</b>: Optional filter to include Todos due on or before this date.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing a filtered and paginated list of Todo items,
        /// each including properties like Title, Description, DueDate, Priority, and Status.
        /// </returns>
        /// <response code="200">Todos fetched successfully.</response>
        /// <response code="404">No Todos found matching the given filters.</response>

        [HttpGet("getAllTodos")]
        public async Task<ActionResult<ApiResponse>> GetAllTodos([FromQuery] PaginationDto pagination)
        {
            _logger.LogInformation("Fetching Todos with filters: Status={Status}, Priority={Priority}, DueDateFrom={DueDateFrom}, DueDateTo={DueDateTo}",
                pagination.Status, pagination.Priority, pagination.DueDateFrom, pagination.DueDateTo);

            Expression<Func<Todo, bool>>? filter = x =>
                (pagination.Status == null || x.Status == pagination.Status) &&
                (pagination.Priority == null || x.Priority == pagination.Priority) &&
                (pagination.DueDateFrom == null || x.DueDate >= pagination.DueDateFrom) &&
                (pagination.DueDateTo == null || x.DueDate <= pagination.DueDateTo);

            var response = await _todoService.GetAllTodosAsync(filter, pagination);

            if (response == null)
            {
                _logger.LogWarning("No Todos found with the given filters");
                return NotFound(new ApiResponse
                {
                    Message = "No Todos found",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }

            _logger.LogInformation("Todos fetched successfully with filters");
            return Ok(new ApiResponse
            {
                Message = "Todos fetched successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }

        /// <summary>
        /// Retrieves all Todo items that contain the specified title text, with optional pagination.
        /// </summary>
        /// <param name="title">The title text to search for (case-insensitive).</param>
        /// <param name="pagination">
        /// The pagination parameters:
        /// <list type="bullet">
        /// <item><description><b>PageNumber</b>: The page number for pagination.</description></item>
        /// <item><description><b>PageSize</b>: The number of items per page.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing a list of matching Todo items, or a not found response if none match.
        /// </returns>
        /// <response code="200">Todos with the specified title were fetched successfully.</response>
        /// <response code="404">No Todos found with the specified title.</response>

        [HttpGet("getTodosByTitle/{title}")]
        public async Task<ActionResult<ApiResponse>> GetTodosByTitle(string title, [FromQuery] PaginationDto pagination)
        {
            _logger.LogInformation("Fetching Todos with Title: {Title}", title);
            var response = await _todoService.GetAllTodosAsync(x => x.Title.ToUpper().Contains(title.ToUpper()), pagination);
            if (response == null)
            {
                _logger.LogError("No Todos found with Title: {Title}", title);
                return NotFound(new ApiResponse
                {
                    Message = "No Todos found",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            _logger.LogInformation("Todos with Title: {Title} fetched successfully", title);
            return Ok(new ApiResponse
            {
                Message = "Todos fetched successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }
        /// <summary>
        /// Retrieves all Todo items that contain the specified description text, with optional pagination.
        /// </summary>
        /// <param name="description">The description text to search for (case-insensitive).</param>
        /// <param name="pagination">
        /// The pagination parameters:
        /// <list type="bullet">
        /// <item><description><b>PageNumber</b>: The page number for pagination.</description></item>
        /// <item><description><b>PageSize</b>: The number of items per page.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing a list of matching Todo items, or a not found response if none match.
        /// </returns>
        /// <response code="200">Todos with the specified description were fetched successfully.</response>
        /// <response code="404">No Todos found with the specified description.</response>

        [HttpGet("getTodosByDescription/{description}")]
        public async Task<ActionResult<ApiResponse>> GetTodosByDescription(string description, [FromQuery] PaginationDto pagination)
        {
            _logger.LogInformation("Fetching Todos with Description: {Description}", description);
            var response = await _todoService.GetAllTodosAsync(x => x.Description.ToUpper().Contains(description.ToUpper()), pagination);
            if (response == null)
            {
                _logger.LogError("No Todos found with Description: {Description}", description);
                return NotFound(new ApiResponse
                {
                    Message = "No Todos found",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            _logger.LogInformation("Todos with Description: {Description} fetched successfully", description);
            return Ok(new ApiResponse
            {
                Message = "Todos fetched successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }
       
        /// <summary>
        /// Marks a Todo item as complete.
        /// </summary>
        /// <param name="id">The ID of the Todo item to mark as complete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the operation, updating Status to Completed (enum <see cref="TodoStatus"/>).</returns>
        /// <response code="200">Todo marked as complete successfully.</response>
        /// <response code="404">Failed to mark Todo as complete.</response>
        [HttpPatch("marktodoComplete/{id}")]
        public async Task<ActionResult<ApiResponse>> MarkTodoComplete(Guid id)
        {
            _logger.LogInformation("Marking Todo with ID: {Id} as complete", id);
            var response = await _todoService.MarkAsCompletedAsync(id);
            if (response == false)
            {
                _logger.LogError("Failed to mark Todo with ID: {Id} as complete", id);
                return NotFound(new ApiResponse
                {
                    Message = "Failed to mark Todo as complete",
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            _logger.LogInformation("Todo with ID: {Id} marked as complete successfully", id);
            return Ok(new ApiResponse
            {
                Message = "Todo marked as complete successfully",
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            });
        }
    }
}