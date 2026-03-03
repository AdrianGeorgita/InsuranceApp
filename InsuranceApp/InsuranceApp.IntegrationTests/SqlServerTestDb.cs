using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InsuranceApp.IntegrationTests;

public class SqlServerTestDb : IAsyncLifetime
{
    private readonly string _masterConnectionString;
    public string DatabaseName { get; } =
        "InsuranceApp_Test_" + Guid.NewGuid().ToString("N");
    public string ConnectionString { get; }

    public SqlServerTestDb()
    {
        Env.TraversePath().Load();
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            config.GetConnectionString("Default")
            ?? config["ConnectionStrings__Default"]
            ?? throw new InvalidOperationException(
                "Missing ConnectionStrings__Default environment variable");

        var masterBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master"
        };
        _masterConnectionString = masterBuilder.ToString();

        var testDbBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = DatabaseName
        };
        ConnectionString = testDbBuilder.ToString();
    }

    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();

        DbMigrator.DbMigrator.Migrate(ConnectionString);
    }

    public async Task DisposeAsync() => await DropDatabaseAsync();

    private async Task CreateDatabaseAsync()
    {
        using var con = new SqlConnection(_masterConnectionString);
        await con.OpenAsync();

        using var cmd = con.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE [{DatabaseName}]";
        cmd.CommandTimeout = 60;

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task DropDatabaseAsync()
    {
        using var con = new SqlConnection(_masterConnectionString);
        await con.OpenAsync();

        using var cmd = con.CreateCommand();

        cmd.CommandText = $@"
IF DB_ID(N'{DatabaseName}') IS NOT NULL
BEGIN
    ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{DatabaseName}];
END";
        cmd.CommandTimeout = 60;

        await cmd.ExecuteNonQueryAsync();
    }
}