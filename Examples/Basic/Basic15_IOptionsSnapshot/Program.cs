// Basic15: IOptionsSnapshot - Scoped內的設定快照
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Basic15_IOptionsSnapshot;

public class AppConfig { public string Value { get; set; } = ""; }

public class MyService
{
    private readonly AppConfig _config;
    public MyService(IOptionsSnapshot<AppConfig> options) => _config = options.Value;
    public void Show() => Console.WriteLine($"設定值: {_config.Value}");
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic15: IOptionsSnapshot ===\n");
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AppConfig:Value"] = "Test" })
            .Build();

        var services = new ServiceCollection();
        services.Configure<AppConfig>(config.GetSection("AppConfig"));
        services.AddScoped<MyService>();

        var provider = services.BuildServiceProvider();
        using (var scope = provider.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<MyService>();
            service.Show();
        }

        Console.WriteLine("\n✅ IOptionsSnapshot 在同一 Scope 內是相同快照");
    }
}
