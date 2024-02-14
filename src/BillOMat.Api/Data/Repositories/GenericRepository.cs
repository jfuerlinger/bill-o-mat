using BillOMat.Api.Data.Specifications;
using BillOMat.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillOMat.Api.Data.Repositories
{
    public class GenericRepository<TEntity>(ApplicationDbContext dbContext)
        : IGenericRepository<TEntity>
        where TEntity : EntityBase
    {
        protected ApplicationDbContext DbContext => dbContext;

        public void Add(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
        }

        public async Task<TEntity[]> GetEntitiesAsync(
            Specification<TEntity> specification,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>()
                .GetQuery(specification)
                .ToArrayAsync(cancellationToken);
        }
    }
}
