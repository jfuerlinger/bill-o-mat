using FluentAssertions;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit.Categories;

namespace BillOMat.Api.IntegrationTests.Features.Patients.CreatePatientTests;

[Collection("Integration Tests with TestContainers")]
public sealed class WithConnectionTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
        => await _msSqlContainer.StartAsync();

    public async Task DisposeAsync()
        => await _msSqlContainer.DisposeAsync();

    [Fact]
    [IntegrationTest]
    public async Task ReadFromMsSqlDatabase()
    {
        await using var connection = new SqlConnection(_msSqlContainer.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1;";

        var actual = await command.ExecuteScalarAsync() as int?;
        actual.GetValueOrDefault()
            .Should()
            .Be(1);
    }
}