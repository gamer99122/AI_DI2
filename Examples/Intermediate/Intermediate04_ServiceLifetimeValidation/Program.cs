// Intermediate04: 生命週期驗證
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intermediate04_ServiceLifetimeValidation;

// Scoped 服務
public interface IScopedService
{
    Guid Id { get; }
}

public class ScopedService : IScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

// ❌ 錯誤：Singleton 注入 Scoped
public class WrongSingleton
{
    private readonly IScopedService _scoped;

    public WrongSingleton(IScopedService scoped) // 這會被驗證器捕捉
    {
        _scoped = scoped;
    }
}

// ✅ 正確：使用 IServiceScopeFactory
public class CorrectSingleton
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CorrectSingleton(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void DoWork()
    {
        using var scope = _scopeFactory.CreateScope();
        var scoped = scope.ServiceProvider.GetRequiredService<IScopedService>();
        Console.WriteLine($"使用 Scoped 服務: {scoped.Id}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Intermediate04: 生命週期驗證 ===\n");

        Console.WriteLine("--- 錯誤示範（會拋出例外）---");
        try
        {
            var host1 = Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = true;  // ✅ 啟用驗證
                    options.ValidateOnBuild = true; // ✅ 建置時驗證
                })
                .ConfigureServices(services =>
                {
                    services.AddScoped<IScopedService, ScopedService>();
                    services.AddSingleton<WrongSingleton>(); // ❌ 這會失敗
                })
                .Build();

            Console.WriteLine("如果看到這行，代表驗證失敗");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✅ 捕捉到錯誤: {ex.Message}");
            Console.WriteLine("驗證器正確地檢測到 Singleton → Scoped 錯誤\n");
        }

        Console.WriteLine("--- 正確示範 ---");
        var host2 = Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices(services =>
            {
                services.AddScoped<IScopedService, ScopedService>();
                services.AddSingleton<CorrectSingleton>(); // ✅ 使用 Factory
            })
            .Build();

        var correct = host2.Services.GetRequiredService<CorrectSingleton>();
        correct.DoWork();

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 開發環境啟用 ValidateScopes");
        Console.WriteLine("✅ 啟用 ValidateOnBuild 提早發現問題");
        Console.WriteLine("❌ Singleton 不能直接注入 Scoped");
        Console.WriteLine("✅ 使用 IServiceScopeFactory 解決");
    }
}
