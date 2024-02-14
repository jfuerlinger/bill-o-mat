using Microsoft.EntityFrameworkCore;
using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public class PatientRepository(ApplicationDbContext dbContext) 
        : GenericRepository<Patient>(dbContext: dbContext), 
        IPatientRepository    
    {
        public async Task<bool> IsEmailUniqueAsync(
            string email, 
            CancellationToken cancellationToken = default)
        {
            return !(await DbContext.Patients
                .AnyAsync(
                p => p.Email == email, 
                cancellationToken));
        }
    }
}
