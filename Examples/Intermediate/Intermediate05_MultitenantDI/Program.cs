// Intermediate05: 多租戶架構
using Microsoft.Extensions.DependencyInjection;

namespace Intermediate05_MultitenantDI;

// 租戶上下文
public interface ITenantContext
{
    string TenantId { get; set; }
}

public class TenantContext : ITenantContext
{
    public string TenantId { get; set; } = "";
}

// 租戶特定服務
public interface ITenantService
{
    void DoWork();
}

public class TenantAService : ITenantService
{
    public void DoWork() => Console.WriteLine("[租戶A] 執行工作");
}

public class TenantBService : ITenantService
{
    public void DoWork() => Console.WriteLine("[租戶B] 執行工作");
}

// 租戶服務工廠
public interface ITenantServiceFactory
{
    ITenantService GetService(string tenantId);
}

public class TenantServiceFactory : ITenantServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TenantServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITenantService GetService(string tenantId)
    {
        return tenantId switch
        {
            "tenant-a" => _serviceProvider.GetRequiredService<TenantAService>(),
            "tenant-b" => _serviceProvider.GetRequiredService<TenantBService>(),
            _ => throw new ArgumentException($"Unknown tenant: {tenantId}")
        };
    }
}

// 應用服務
public class ApplicationService
{
    private readonly ITenantContext _tenantContext;
    private readonly ITenantServiceFactory _factory;

    public ApplicationService(ITenantContext tenantContext, ITenantServiceFactory factory)
    {
        _tenantContext = tenantContext;
        _factory = factory;
    }

    public void ProcessRequest()
    {
        Console.WriteLine($"處理租戶 '{_tenantContext.TenantId}' 的請求");
        var service = _factory.GetService(_tenantContext.TenantId);
        service.DoWork();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Intermediate05: 多租戶架構 ===\n");

        var services = new ServiceCollection();

        // 租戶上下文（每個請求一個）
        services.AddScoped<ITenantContext, TenantContext>();

        // 租戶特定服務
        services.AddTransient<TenantAService>();
        services.AddTransient<TenantBService>();

        // 工廠
        services.AddSingleton<ITenantServiceFactory, TenantServiceFactory>();

        // 應用服務
        services.AddScoped<ApplicationService>();

        var provider = services.BuildServiceProvider();

        // 模擬租戶A的請求
        Console.WriteLine("--- 請求 1 (租戶A) ---");
        using (var scope1 = provider.CreateScope())
        {
            var context = scope1.ServiceProvider.GetRequiredService<ITenantContext>();
            context.TenantId = "tenant-a";

            var appService = scope1.ServiceProvider.GetRequiredService<ApplicationService>();
            appService.ProcessRequest();
        }

        // 模擬租戶B的請求
        Console.WriteLine("\n--- 請求 2 (租戶B) ---");
        using (var scope2 = provider.CreateScope())
        {
            var context = scope2.ServiceProvider.GetRequiredService<ITenantContext>();
            context.TenantId = "tenant-b";

            var appService = scope2.ServiceProvider.GetRequiredService<ApplicationService>();
            appService.ProcessRequest();
        }

        Console.WriteLine("\n✅ 每個租戶使用獨立的服務實作");
        Console.WriteLine("✅ 使用 Scoped 確保請求內租戶一致");
        Console.WriteLine("✅ 工廠模式動態選擇實作");
    }
}
