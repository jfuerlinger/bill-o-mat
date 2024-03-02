using Microsoft.AspNetCore.Mvc.Testing;

namespace BillOMat.Api.IntegrationTests
{
    public class BillOMatWebApplicationFactory<TProgram>(string connectionString)
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        override protected void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var configuration = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ApplicationDbContext"] = connectionString,
                };

                config.Sources.Clear();
                config.AddInMemoryCollection(configuration);
            });

            builder.UseEnvironment("Development");
        }
        
    }
}
