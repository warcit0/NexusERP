namespace NexusERP.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : Domain.Entities.BaseEntity;
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
