// Advanced01: 自訂 ServiceProvider
using Microsoft.Extensions.DependencyInjection;

namespace Advanced01_CustomServiceProvider;

// 記錄所有服務解析的自訂 ServiceProvider
public class LoggingServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _inner;

    public LoggingServiceProvider(IServiceProvider inner)
    {
        _inner = inner;
    }

    public object? GetService(Type serviceType)
    {
        Console.WriteLine($"[Provider] 解析服務: {serviceType.Name}");
        var service = _inner.GetService(serviceType);

        if (service != null)
        {
            Console.WriteLine($"[Provider] ✅ 解析成功");
        }
        else
        {
            Console.WriteLine($"[Provider] ❌ 服務不存在");
        }

        return service;
    }
}

// 自訂 ServiceProviderFactory
public class LoggingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        var defaultProvider = containerBuilder.BuildServiceProvider();
        return new LoggingServiceProvider(defaultProvider);
    }
}

// 測試服務
public interface IMyService
{
    void Execute();
}

public class MyService : IMyService
{
    public void Execute() => Console.WriteLine("[MyService] 執行中...\n");
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Advanced01: 自訂 ServiceProvider ===\n");

        var services = new ServiceCollection();
        services.AddTransient<IMyService, MyService>();

        // 使用自訂 Factory
        var factory = new LoggingServiceProviderFactory();
        var provider = factory.CreateServiceProvider(services);

        Console.WriteLine("--- 第一次解析 ---");
        var service1 = provider.GetService<IMyService>();
        service1?.Execute();

        Console.WriteLine("--- 第二次解析 ---");
        var service2 = provider.GetService<IMyService>();
        service2?.Execute();

        Console.WriteLine("--- 解析不存在的服務 ---");
        var service3 = provider.GetService<IDisposable>();

        Console.WriteLine("\n✅ 自訂 ServiceProvider 可以:");
        Console.WriteLine("  - 記錄服務解析");
        Console.WriteLine("  - 效能監控");
        Console.WriteLine("  - 安全檢查");
        Console.WriteLine("  - 代理注入");
    }
}
