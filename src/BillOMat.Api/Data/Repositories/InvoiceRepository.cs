using BillOMat.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillOMat.Api.Data.Repositories
{
    public class InvoiceRepository(ApplicationDbContext dbContext)
        : GenericRepository<Invoice>(dbContext),
            IInvoiceRepository
    {
        public async Task<bool> IsInvoiceNumberUniqueAsync(
            string invoiceNumber, 
            CancellationToken cancellationToken)
        {
                return !(await DbContext.Invoices
                    .AnyAsync(
                        i => i.InvoiceNumber == invoiceNumber,
                        cancellationToken));
        }
    }
}
