// ========================================
// Basic04: Scoped 生命週期
// ========================================
// 特性：同一範圍內共享實例，不同範圍創建新實例
// 使用：Web 請求、DbContext、UnitOfWork

using Microsoft.Extensions.DependencyInjection;

namespace Basic04_ScopedLifetime;

public interface IRequestContext
{
    Guid RequestId { get; }
    DateTime CreatedAt { get; }
}

public class RequestContext : IRequestContext
{
    public Guid RequestId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.Now;
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic04: Scoped 生命週期 ===\n");

        var services = new ServiceCollection();

        // ✅ AddScoped: 同一範圍內共享
        services.AddScoped<IRequestContext, RequestContext>();

        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--- Scope 1 ---");
        using (var scope1 = serviceProvider.CreateScope())
        {
            var ctx1 = scope1.ServiceProvider.GetRequiredService<IRequestContext>();
            var ctx2 = scope1.ServiceProvider.GetRequiredService<IRequestContext>();

            Console.WriteLine($"實例1 ID: {ctx1.RequestId}");
            Console.WriteLine($"實例2 ID: {ctx2.RequestId}");
            Console.WriteLine($"是否相同？{ReferenceEquals(ctx1, ctx2)} ✅ 應該是 True");
        }

        Console.WriteLine("\n--- Scope 2 ---");
        using (var scope2 = serviceProvider.CreateScope())
        {
            var ctx = scope2.ServiceProvider.GetRequiredService<IRequestContext>();
            Console.WriteLine($"新範圍的 ID: {ctx.RequestId}");
            Console.WriteLine("✅ 不同範圍有不同實例");
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 同一 Scope 內是同一個實例");
        Console.WriteLine("✅ 不同 Scope 是不同實例");
        Console.WriteLine("✅ ASP.NET Core 每個 HTTP 請求就是一個 Scope");
        Console.WriteLine("✅ DbContext 應該用 Scoped");
    }
}
