using BillOMat.Api.Data.Repositories;

namespace BillOMat.Api.Data
{
    public interface IUnitOfWork
    {
        IPatientRepository PatientRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
