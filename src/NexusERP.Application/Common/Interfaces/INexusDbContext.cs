using NexusERP.Domain.Entities;

namespace NexusERP.Application.Common.Interfaces;

public interface INexusDbContext
{
    // dbsets irán aquí

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

