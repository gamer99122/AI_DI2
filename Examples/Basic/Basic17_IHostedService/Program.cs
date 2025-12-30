// Basic17: 背景服務
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basic17_IHostedService;

public class MyBackgroundService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[BackgroundService] 啟動");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[BackgroundService] 停止");
        return Task.CompletedTask;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Basic17: IHostedService ===\n");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<MyBackgroundService>();
            })
            .Build();

        await host.StartAsync();
        Console.WriteLine("✅ IHostedService 用於背景任務");
        await host.StopAsync();
    }
}
