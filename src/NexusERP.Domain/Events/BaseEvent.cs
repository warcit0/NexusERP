using MediatR;

namespace NexusERP.Domain.Events;

/// <summary>
/// Clase base para todos los eventos de dominio.
/// Implementa INotification de MediatR para publicación de eventos.
/// </summary>
public abstract class BaseEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
