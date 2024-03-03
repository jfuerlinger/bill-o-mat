using BillOMat.Api.Entities;
using BillOMat.Api.Features.Invoices;
using Riok.Mapperly.Abstractions;

namespace BillOMat.Api.Mappers
{
    [Mapper]
    public partial class InvoiceMapper
    {
        public partial ListInvoiceDto InvoiceToListInvoicesDto(Invoice invoice);
    }
}
