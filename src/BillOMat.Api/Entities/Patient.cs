namespace BillOMat.Api.Entities
{
    public class Patient : EntityBase
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Nickname { get; set; }
        public required string Email { get; set; }
        public List<Invoice> Invoices { get; set; } = [];

        public void AddInvoice(Invoice invoice)
        {
            Invoices.Add(invoice);
        }
    }
}
