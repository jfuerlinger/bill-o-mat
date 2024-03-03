using BillOMat.Api.Features.Patients;
using FluentAssertions;
using System.Diagnostics;
using System.Net;
using Testcontainers.MsSql;
using Xunit.Categories;

namespace BillOMat.Api.IntegrationTests.Features.Patients.CreatePatientTests;

[Collection("Integration Tests with TestContainers")]
public sealed class WithValidCommand : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithAutoRemove(false)
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
        => await _msSqlContainer.StartAsync();

    public async Task DisposeAsync()
        => await _msSqlContainer.DisposeAsync();

    [Fact]
    [IntegrationTest]
    public async Task CreatePatientEndpoint_ValidPatientCreateRequest_PatientShouldBeInTheDatabase()
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
                                FirstName = "Sepp",
                                LastName = "Fürlinger",
                                Nickname = "Opa",
                                Email = "sepp.fuerlinger@gmail.com"
                            });

        // assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
    }
}