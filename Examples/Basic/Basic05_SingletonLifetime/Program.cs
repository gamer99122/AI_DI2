// ========================================
// Basic05: Singleton 生命週期
// ========================================
// 特性：整個應用程式生命週期只有一個實例
// 使用：快取、設定、全域狀態
// 注意：必須是執行緒安全的

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Basic05_SingletonLifetime;

public interface IApplicationCache
{
    void Set(string key, object value);
    object? Get(string key);
    int GetAccessCount();
}

public class ApplicationCache : IApplicationCache
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    private int _accessCount = 0;
    private readonly Guid _instanceId = Guid.NewGuid();

    public ApplicationCache()
    {
        Console.WriteLine($"[Cache] 實例建立: {_instanceId}");
    }

    public void Set(string key, object value)
    {
        Interlocked.Increment(ref _accessCount);
        _cache[key] = value;
        Console.WriteLine($"[Cache] 設定 {key} = {value}");
    }

    public object? Get(string key)
    {
        Interlocked.Increment(ref _accessCount);
        _cache.TryGetValue(key, out var value);
        Console.WriteLine($"[Cache] 取得 {key} = {value}");
        return value;
    }

    public int GetAccessCount() => _accessCount;
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic05: Singleton 生命週期 ===\n");

        var services = new ServiceCollection();

        // ✅ AddSingleton: 全域單一實例
        services.AddSingleton<IApplicationCache, ApplicationCache>();

        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--- 測試 1: 多次請求 ---");
        var cache1 = serviceProvider.GetRequiredService<IApplicationCache>();
        var cache2 = serviceProvider.GetRequiredService<IApplicationCache>();
        var cache3 = serviceProvider.GetRequiredService<IApplicationCache>();

        Console.WriteLine($"是否為同一實例？{ReferenceEquals(cache1, cache2)} ✅");

        Console.WriteLine("\n--- 測試 2: 共享狀態 ---");
        cache1.Set("user:1", "張三");
        cache2.Set("user:2", "李四");

        cache3.Get("user:1"); // cache3 能看到 cache1 設定的值
        cache3.Get("user:2"); // cache3 能看到 cache2 設定的值

        Console.WriteLine($"\n總存取次數: {cache1.GetAccessCount()}");

        Console.WriteLine("\n--- 測試 3: 不同 Scope 也共享 ---");
        using (var scope = serviceProvider.CreateScope())
        {
            var cacheInScope = scope.ServiceProvider.GetRequiredService<IApplicationCache>();
            Console.WriteLine($"Scope 內也是同一實例？{ReferenceEquals(cache1, cacheInScope)} ✅");
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 整個應用程式只有一個實例");
        Console.WriteLine("✅ 所有地方共享狀態");
        Console.WriteLine("⚠️  必須是執行緒安全的（使用 ConcurrentDictionary, Interlocked）");
        Console.WriteLine("⚠️  不要注入 Scoped 服務到 Singleton");
        Console.WriteLine("✅ 適用：快取、設定、Logger Factory");
    }
}
