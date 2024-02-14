using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<bool> IsInvoiceNumberUniqueAsync(
            string invoiceNumber, 
            CancellationToken cancellationToken);
    }
}
