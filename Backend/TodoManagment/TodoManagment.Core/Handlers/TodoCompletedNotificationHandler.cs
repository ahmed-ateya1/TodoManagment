using MediatR;
using TodoManagment.Core.Domain.Events;
using TodoManagment.Core.ServiceContract;

namespace TodoManagment.Core.Handlers
{
    public class TodoCompletedNotificationHandler : INotificationHandler<TodoCompletedEvent>
    {
        private readonly INotificationService _notificationService;

        public TodoCompletedNotificationHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                $"Todo '{notification.Todo.Title}' was completed!",
                $"The todo item '{notification.Todo.Title}' was marked as complete at {notification.OccurredOn}");
        }
    }
}
