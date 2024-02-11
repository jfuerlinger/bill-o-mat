using BillOMat.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillOMat.Api.Data.Specifications
{
    public static class SpecificationQueryBuilder
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(
            this IQueryable<TEntity> inputQuery,
            Specification<TEntity> specification)
            where TEntity : EntityBase
        {
            var query = inputQuery;

            if (specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.Includes.Count > 0)
            {
                query = specification
                            .Includes
                                .Aggregate(
                                    query,
                                    (current, include) => current.Include(include));
            }

            if (specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            return query;
        }
    }
}
