// Intermediate01: Scoped 服務工廠模式
using Microsoft.Extensions.DependencyInjection;

namespace Intermediate01_ScopedFactory;

// Scoped 服務
public interface IDbContext
{
    Guid InstanceId { get; }
    void SaveChanges();
}

public class MyDbContext : IDbContext
{
    public Guid InstanceId { get; } = Guid.NewGuid();

    public void SaveChanges()
    {
        Console.WriteLine($"[DbContext {InstanceId}] 保存變更");
    }
}

// Singleton 背景服務需要使用 Scoped 服務
public class BackgroundWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void DoWork()
    {
        Console.WriteLine("\n[Worker] 開始工作...");

        // ✅ 為每個工作單元創建新的 Scope
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            Console.WriteLine($"[Worker] 使用 DbContext: {dbContext.InstanceId}");

            // 模擬資料庫操作
            dbContext.SaveChanges();
        }

        Console.WriteLine("[Worker] 工作完成，Scope 已釋放");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Intermediate01: Scoped 服務工廠 ===\n");

        var services = new ServiceCollection();

        // 註冊 Scoped 服務
        services.AddScoped<IDbContext, MyDbContext>();

        // 註冊 Singleton 服務
        services.AddSingleton<BackgroundWorker>();

        var provider = services.BuildServiceProvider();
        var worker = provider.GetRequiredService<BackgroundWorker>();

        // 執行多次工作，每次都會創建新的 Scope 和 DbContext
        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"--- 工作 {i} ---");
            worker.DoWork();
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ Singleton 不能直接注入 Scoped 服務");
        Console.WriteLine("✅ 使用 IServiceScopeFactory 創建 Scope");
        Console.WriteLine("✅ 每個 Scope 都有獨立的 Scoped 服務實例");
        Console.WriteLine("✅ 適用於背景服務、定時任務");
    }
}
