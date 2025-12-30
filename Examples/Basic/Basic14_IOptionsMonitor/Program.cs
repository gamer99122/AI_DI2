// Basic14: IOptionsMonitor 動態設定
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Basic14_IOptionsMonitor;

public class AppSettings
{
    public int MaxRetry { get; set; }
    public int Timeout { get; set; }
}

public class Worker
{
    private readonly IOptionsMonitor<AppSettings> _monitor;

    public Worker(IOptionsMonitor<AppSettings> monitor)
    {
        _monitor = monitor;
        // 監聽設定變更
        _monitor.OnChange(settings =>
        {
            Console.WriteLine($"[變更] MaxRetry={settings.MaxRetry}, Timeout={settings.Timeout}");
        });
    }

    public void ShowSettings()
    {
        var settings = _monitor.CurrentValue;
        Console.WriteLine($"MaxRetry: {settings.MaxRetry}");
        Console.WriteLine($"Timeout: {settings.Timeout}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic14: IOptionsMonitor ===\n");

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:MaxRetry"] = "3",
                ["AppSettings:Timeout"] = "30"
            })
            .Build();

        var services = new ServiceCollection();
        services.Configure<AppSettings>(config.GetSection("AppSettings"));
        services.AddSingleton<Worker>();

        var provider = services.BuildServiceProvider();
        var worker = provider.GetRequiredService<Worker>();

        worker.ShowSettings();

        Console.WriteLine("\n✅ IOptionsMonitor 可監聽設定變更");
        Console.WriteLine("✅ CurrentValue 總是最新值");
    }
}
