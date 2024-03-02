using BillOMat.Api.Features.Patients;
using FluentAssertions;
using System.Diagnostics;
using System.Net;
using Testcontainers.MsSql;

namespace BillOMat.Api.IntegrationTests.Features.Patients.CreatePatientTests;
public sealed class WithDuplicateEmail : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
        => await _msSqlContainer.StartAsync();

    public async Task DisposeAsync()
        => await _msSqlContainer.DisposeAsync();


    [Fact]
    public async Task CreatePatientEndpoint_WithExistingEmailAWithExistingEmailAddressRShouldBeReturnBadRequest()
    {
        // arrange
        string connectionString = _msSqlContainer.GetConnectionString();
        Debug.WriteLine(connectionString);

        var webAppFactory =
            new BillOMatWebApplicationFactory<Program>(
                connectionString);

        var client = webAppFactory.CreateClient();

        // act
        var response = await client.PostAsJsonAsync(
                           "/patients?api-version=1",
                            new CreatePatient.Command
                            {
                                FirstName = "Joe-Test",
                                LastName = "Fürlinger",
                                Nickname = "Joe-Test",
                                Email = "josef.fuerlinger@gmail.com"
                            });

        // assert
        response.StatusCode
            .Should()
                .Be(HttpStatusCode.BadRequest);
    }
}