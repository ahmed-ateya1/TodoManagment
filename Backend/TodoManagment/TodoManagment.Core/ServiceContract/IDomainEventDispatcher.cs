using TodoManagment.Core.Domain.Entities.Common;

namespace TodoManagment.Core.ServiceContract
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(DomainEvent domainEvent);
    }
}
