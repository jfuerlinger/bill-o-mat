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
    public async Task CreatePatientEndpoint_ValidPatientCreateRequest_ResultShouldBeHttpCreated()
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
                                LastName = "F�rlinger",
                                Nickname = "Opa",
                                Email = "sepp.fuerlinger@gmail.com"
                            });

        // assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
    }

    [Fact]
    [IntegrationTest]
    public async Task CreatePatientEndpoint_ValidPatientCreateRequest_PatientShouldBeDeliveredByGet()
    {
        // arrange
        string connectionString = _msSqlContainer.GetConnectionString();
        Debug.WriteLine(connectionString);

        var webAppFactory =
            new BillOMatWebApplicationFactory<Program>(
                connectionString);

        var client = webAppFactory.CreateClient();

        // act
        var createResponse = await client.PostAsJsonAsync(
                           "/patients?api-version=1",
                            new CreatePatient.Command
                            {
                                FirstName = "Annemarie",
                                LastName = "F�rlinger",
                                Nickname = "Oma",
                                Email = "annemarie.fuerlinger@gmail.com"
                            });

        var createdPatientId = Convert.ToInt32(
                                        await createResponse
                                                .Content
                                                .ReadAsStringAsync());

        var response = await client.GetAsync($"/patients/{createdPatientId}?api-version=1");

        // assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}