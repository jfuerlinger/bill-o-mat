namespace BillOMat.Api.Entities
{
    public class Patient : EntityBase
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public required string Nickname { get; set; }
        public string? Email { get; set; }
    }
}
