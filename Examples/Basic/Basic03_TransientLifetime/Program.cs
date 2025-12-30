// ========================================
// Basic03: Transient 生命週期
// ========================================
// 目的：理解 Transient 生命週期
// - 每次請求都創建新實例
// - 適合無狀態、輕量級的服務

using Microsoft.Extensions.DependencyInjection;

namespace Basic03_TransientLifetime;

/// <summary>
/// GUID 產生器服務
/// </summary>
public interface IGuidGenerator
{
    Guid InstanceId { get; }  // 實例的唯一識別碼
    Guid Generate();           // 產生新的 GUID
}

public class GuidGenerator : IGuidGenerator
{
    // 每個實例都有自己的 InstanceId
    public Guid InstanceId { get; } = Guid.NewGuid();

    public Guid Generate()
    {
        return Guid.NewGuid();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Basic03: Transient 生命週期");
        Console.WriteLine("========================================\n");

        // 建立服務容器
        var services = new ServiceCollection();

        // ✅ 使用 AddTransient 註冊服務
        // Transient = 每次請求都創建新實例
        services.AddTransient<IGuidGenerator, GuidGenerator>();

        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--- 測試 1: 連續請求三次 ---");
        for (int i = 1; i <= 3; i++)
        {
            var generator = serviceProvider.GetRequiredService<IGuidGenerator>();
            Console.WriteLine($"第 {i} 次請求：");
            Console.WriteLine($"  實例 ID: {generator.InstanceId}");
            Console.WriteLine($"  產生的 GUID: {generator.Generate()}");
            Console.WriteLine();
        }

        Console.WriteLine("--- 測試 2: 同時持有多個實例 ---");
        var gen1 = serviceProvider.GetRequiredService<IGuidGenerator>();
        var gen2 = serviceProvider.GetRequiredService<IGuidGenerator>();
        var gen3 = serviceProvider.GetRequiredService<IGuidGenerator>();

        Console.WriteLine($"實例 1 ID: {gen1.InstanceId}");
        Console.WriteLine($"實例 2 ID: {gen2.InstanceId}");
        Console.WriteLine($"實例 3 ID: {gen3.InstanceId}");

        Console.WriteLine($"\n三個實例是否相同？");
        Console.WriteLine($"gen1 == gen2: {ReferenceEquals(gen1, gen2)} (應該是 False)");
        Console.WriteLine($"gen2 == gen3: {ReferenceEquals(gen2, gen3)} (應該是 False)");

        Console.WriteLine("\n========================================");
        Console.WriteLine("Transient 生命週期特性：");
        Console.WriteLine("✅ 每次 GetService 都創建新實例");
        Console.WriteLine("✅ 適合無狀態的服務");
        Console.WriteLine("✅ 輕量級物件");
        Console.WriteLine("⚠️  如果物件創建成本高，不建議使用");
        Console.WriteLine("⚠️  實作 IDisposable 的物件會被容器追蹤");
        Console.WriteLine("========================================");
    }
}
