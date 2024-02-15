namespace BillOMat.Api.Data.Exceptions
{
    public class ConnectionStringNotFoundException(string connectionStringName) 
        : Exception(
            $"Connection string '{connectionStringName}' was not found!")
    {
        public string ConnectionStringName { get; set; } = connectionStringName;
    }
}
