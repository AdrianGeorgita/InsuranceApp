namespace InsuranceApp.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; set; } = null!;
    protected SqlServerTestDb Db { get; set; } = null!;

    public async Task InitializeAsync()
    {
        Db = new SqlServerTestDb();
        await Db.InitializeAsync();

        var factory = new InsuranceAppWebApplicationFactory(Db.ConnectionString);
        HttpClient = factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Db.DisposeAsync();
    }
}