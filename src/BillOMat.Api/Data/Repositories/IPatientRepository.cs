using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    }
}
