using MediatR;
using Microsoft.Extensions.Logging;
using TodoManagment.Core.Domain.Events;

namespace TodoManagment.Core.Handlers
{
    public class TodoCompletedEventHandler : INotificationHandler<TodoCompletedEvent>
    {
        private readonly ILogger<TodoCompletedEventHandler> _logger;

        public TodoCompletedEventHandler(ILogger<TodoCompletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Todo with ID {TodoId} was completed at {CompletionTime}",
                notification.Todo.Id, notification.OccurredOn);


            return Task.CompletedTask;
        }
    }
}
