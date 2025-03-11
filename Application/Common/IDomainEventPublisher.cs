using Domain.Primitives;

namespace Application.Common
{
    public interface IDomainEventPublisher
    {
        Task PublishEventAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
    }
}
