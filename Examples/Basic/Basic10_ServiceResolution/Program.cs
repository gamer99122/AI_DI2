// Basic10: 服務解析方式
using Microsoft.Extensions.DependencyInjection;

namespace Basic10_ServiceResolution;

public interface IMyService { void DoWork(); }
public class MyService : IMyService
{
    public void DoWork() => Console.WriteLine("執行工作");
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic10: 服務解析 ===\n");

        var services = new ServiceCollection();
        services.AddTransient<IMyService, MyService>();
        var provider = services.BuildServiceProvider();

        // 方式1: GetService (可能回傳 null)
        var service1 = provider.GetService<IMyService>();
        if (service1 != null)
        {
            Console.WriteLine("✅ GetService 成功");
            service1.DoWork();
        }

        // 方式2: GetRequiredService (不存在會拋例外)
        var service2 = provider.GetRequiredService<IMyService>();
        Console.WriteLine("✅ GetRequiredService 成功");
        service2.DoWork();

        // 方式3: 取得多個實作
        services.AddTransient<IMyService, MyService>();
        services.AddTransient<IMyService, MyService>();
        provider = services.BuildServiceProvider();

        var allServices = provider.GetServices<IMyService>();
        Console.WriteLine($"\n取得 {allServices.Count()} 個服務實例");

        Console.WriteLine("\n建議：");
        Console.WriteLine("✅ 必要服務用 GetRequiredService");
        Console.WriteLine("⚠️ 可選服務用 GetService");
        Console.WriteLine("📦 多個實作用 GetServices");
    }
}
