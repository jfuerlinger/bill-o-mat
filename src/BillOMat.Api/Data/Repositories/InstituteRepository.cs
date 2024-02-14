using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public class InstituteRepository(ApplicationDbContext dbContext) 
        : GenericRepository<Institute>(dbContext), IInstituteRepository
    {
    }
}
