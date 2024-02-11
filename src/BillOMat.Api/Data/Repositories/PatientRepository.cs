using Microsoft.EntityFrameworkCore;
using BillOMat.Api.Data.Specifications;
using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public class PatientRepository(ApplicationDbContext dbContext)
        : IPatientRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<Patient[]> GetEntitiesAsync(Specification<Patient> specification)
        {
            return _dbContext.Patients.GetQuery(specification)
                .ToArrayAsync();
        }
    }
}
