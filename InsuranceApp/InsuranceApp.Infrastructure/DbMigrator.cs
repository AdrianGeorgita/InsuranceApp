using System.Reflection;
using DbUp;

namespace InsuranceApp.DbMigrator;

public static class DbMigrator
{
    public static int Migrate(string connectionString)
    {
        Console.WriteLine("Running migrations...");
        var assembly = Assembly.GetExecutingAssembly();
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(assembly, s => s.StartsWith($"{assembly.GetName().Name}.Migrations.", StringComparison.OrdinalIgnoreCase)
                                                          && s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            Console.WriteLine(result.Error);
#if DEBUG
            Console.ReadLine();
#endif
            return -1;
        }

        Console.WriteLine("Success!");
        return 0;
    }
}