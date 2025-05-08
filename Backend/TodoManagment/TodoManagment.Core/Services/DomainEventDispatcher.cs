using MediatR;
using TodoManagment.Core.Domain.Entities.Common;
using TodoManagment.Core.ServiceContract;

namespace TodoManagment.Core.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Dispatch(DomainEvent domainEvent)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
