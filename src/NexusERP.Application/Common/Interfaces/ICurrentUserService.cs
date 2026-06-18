namespace NexusERP.Application.Common.Interfaces;

/// <summary>
/// Servicio para resolver el usuario actual autenticado.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
}
