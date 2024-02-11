using Microsoft.EntityFrameworkCore;
using BillOMat.Api.Data.Specifications;
using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public class PatientRepository(ApplicationDbContext dbContext)
        : IPatientRepository
    {
        public Task<Patient[]> GetEntitiesAsync(Specification<Patient> specification)
        {
            return dbContext.Patients.GetQuery(specification)
                .ToArrayAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
        {
            return !(await dbContext.Patients
                .AnyAsync(p => p.Email == email, cancellationToken));
        }
    }
}
