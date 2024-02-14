namespace BillOMat.Api.Entities
{
    public class Institute : EntityBase
    {
        public required string Name { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string Postalcode { get; set; }
        public required string City { get; set; }
        public string? Email { get; set; }
        public List<Invoice> Invoices { get; set; } = [];
    }
}
