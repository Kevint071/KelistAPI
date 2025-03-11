namespace Domain.Primitives;

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents;
    protected void Raise(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}