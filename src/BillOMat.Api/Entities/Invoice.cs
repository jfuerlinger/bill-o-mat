namespace BillOMat.Api.Entities
{
    public class Invoice : EntityBase
    {
        public required string InvoiceNumber { get; set; }
        public required Enumerations.Invoice.Status Status { get; set; }

        public required int InstituteId { get; set; }
        public Institute? Institute { get; set; }

        public required int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }

        public DateTime? SentToOegk { get; set; }
        public DateTime? SentToMerkur { get; set; }
    }
}
