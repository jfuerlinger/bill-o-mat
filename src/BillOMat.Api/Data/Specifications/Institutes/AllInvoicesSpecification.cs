using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Specifications.Institutes
{
    public class AllInvoicesSpecification : Specification<Invoice>
    {
        public AllInvoicesSpecification()
        {
            AddOrderByDescending(i => i.InvoiceDate);
        }
    }
}
