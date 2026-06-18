using NexusERP.Domain.Events;

namespace NexusERP.Domain.Entities;

/// <summary>
/// Entidad base para todos los objetos del dominio.
/// Contiene el identificador único y los eventos de dominio.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    private readonly List<BaseEvent> _domainEvents = [];

    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(BaseEvent domainEvent)
        => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
