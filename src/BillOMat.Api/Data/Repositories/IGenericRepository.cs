using BillOMat.Api.Data.Specifications;
using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : EntityBase
    {
        Task<TEntity[]> GetEntitiesAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);

        void Add(TEntity entity);
    }
}