using MediatR;

namespace TodoManagment.Core.Domain.Entities.Common
{
    public abstract class DomainEvent : INotification
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
