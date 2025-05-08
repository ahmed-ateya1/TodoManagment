using MediatR;
using TodoManagment.Core.Domain.Entities;
using TodoManagment.Core.Domain.Entities.Common;

namespace TodoManagment.Core.Domain.Events
{
    public class TodoCompletedEvent : DomainEvent, INotification
    {
        public Todo Todo { get; }

        public TodoCompletedEvent(Todo todo)
        {
            Todo = todo;
        }
    }
}