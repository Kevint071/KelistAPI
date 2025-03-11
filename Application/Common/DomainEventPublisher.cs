using Domain.Primitives;
using MediatR;

namespace Application.Common
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IPublisher _publisher;

        public DomainEventPublisher(IPublisher publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task PublishEventAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in events)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
