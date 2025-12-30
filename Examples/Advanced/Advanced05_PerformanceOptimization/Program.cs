// Advanced05: 效能優化技巧
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;

namespace Advanced05_PerformanceOptimization;

// 重量級物件（創建成本高）
public class HeavyObject
{
    private readonly byte[] _buffer;

    public HeavyObject()
    {
        // 模擬昂貴的初始化
        Thread.Sleep(10);
        _buffer = new byte[1024 * 1024]; // 1MB
        Console.WriteLine("[HeavyObject] 創建（耗時）");
    }

    public void DoWork()
    {
        // 工作邏輯
    }

    public void Reset()
    {
        // 重置狀態以便重用
        Array.Clear(_buffer, 0, _buffer.Length);
    }
}

// 物件池策略
public class HeavyObjectPolicy : IPooledObjectPolicy<HeavyObject>
{
    public HeavyObject Create()
    {
        return new HeavyObject();
    }

    public bool Return(HeavyObject obj)
    {
        obj.Reset();
        return true;
    }
}

// 服務包裝器
public interface IHeavyService
{
    void Execute();
}

public class HeavyService : IHeavyService
{
    private readonly ObjectPool<HeavyObject> _pool;

    public HeavyService(ObjectPool<HeavyObject> pool)
    {
        _pool = pool;
    }

    public void Execute()
    {
        // 從池中租用物件
        var obj = _pool.Get();

        try
        {
            obj.DoWork();
            Console.WriteLine("[HeavyService] 使用池中的物件");
        }
        finally
        {
            // 歸還物件到池
            _pool.Return(obj);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Advanced05: 效能優化 ===\n");

        var services = new ServiceCollection();

        // 方式1：使用 Singleton（不推薦，如果物件有狀態）
        // services.AddSingleton<HeavyObject>();

        // 方式2：使用物件池（推薦）
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            var policy = new HeavyObjectPolicy();
            return provider.Create(policy);
        });

        services.AddScoped<IHeavyService, HeavyService>();

        var provider = services.BuildServiceProvider();

        // 效能測試：使用物件池
        Console.WriteLine("--- 使用物件池 ---");
        var sw1 = Stopwatch.StartNew();

        using (var scope = provider.CreateScope())
        {
            for (int i = 0; i < 10; i++)
            {
                var service = scope.ServiceProvider.GetRequiredService<IHeavyService>();
                service.Execute();
            }
        }

        sw1.Stop();
        Console.WriteLine($"✅ 總耗時: {sw1.ElapsedMilliseconds}ms\n");

        // 效能測試：不使用物件池（對比）
        Console.WriteLine("--- 不使用物件池（對比）---");
        var sw2 = Stopwatch.StartNew();

        for (int i = 0; i < 10; i++)
        {
            var obj = new HeavyObject();
            obj.DoWork();
        }

        sw2.Stop();
        Console.WriteLine($"❌ 總耗時: {sw2.ElapsedMilliseconds}ms\n");

        Console.WriteLine($"效能提升: {(double)sw2.ElapsedMilliseconds / sw1.ElapsedMilliseconds:F2}x");

        Console.WriteLine("\n✅ 效能優化技巧:");
        Console.WriteLine("  1. 使用 Singleton 快取昂貴物件");
        Console.WriteLine("  2. 使用 ObjectPool 重用物件");
        Console.WriteLine("  3. 使用 Lazy<T> 延遲初始化");
        Console.WriteLine("  4. 避免在迴圈中解析服務");
        Console.WriteLine("  5. 選擇合適的生命週期");
    }
}
