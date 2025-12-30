// Basic26: 裝飾者模式
using Microsoft.Extensions.DependencyInjection;

namespace Basic26_DecoratorPattern;

// 資料服務介面
public interface IDataService
{
    string GetData();
}

// 基本實作
public class DataService : IDataService
{
    public string GetData()
    {
        Console.WriteLine("[DataService] 取得資料");
        return "Original Data";
    }
}

// 裝飾者 1: 加入快取
public class CachedDataService : IDataService
{
    private readonly IDataService _inner;
    private string? _cachedData;

    public CachedDataService(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData()
    {
        if (_cachedData != null)
        {
            Console.WriteLine("[CachedDataService] 從快取回傳");
            return _cachedData;
        }

        Console.WriteLine("[CachedDataService] 快取未命中，載入資料");
        _cachedData = _inner.GetData();
        return _cachedData;
    }
}

// 裝飾者 2: 加入日誌
public class LoggingDataService : IDataService
{
    private readonly IDataService _inner;

    public LoggingDataService(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData()
    {
        Console.WriteLine("[LoggingDataService] 開始取得資料");
        var data = _inner.GetData();
        Console.WriteLine($"[LoggingDataService] 資料長度: {data.Length}");
        return data;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic26: 裝飾者模式 ===\n");

        var services = new ServiceCollection();

        // 註冊基本服務
        services.AddTransient<DataService>();

        // 使用裝飾者包裝
        services.AddTransient<IDataService>(sp =>
        {
            var baseService = sp.GetRequiredService<DataService>();
            var cached = new CachedDataService(baseService);
            var logged = new LoggingDataService(cached);
            return logged;
        });

        var provider = services.BuildServiceProvider();

        Console.WriteLine("--- 第一次呼叫 ---");
        var service = provider.GetRequiredService<IDataService>();
        var data1 = service.GetData();
        Console.WriteLine($"結果: {data1}\n");

        Console.WriteLine("--- 第二次呼叫（有快取）---");
        var data2 = service.GetData();
        Console.WriteLine($"結果: {data2}");

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 裝飾者模式動態擴展功能");
        Console.WriteLine("✅ 不修改原始類別");
        Console.WriteLine("✅ 可以層層包裝");
    }
}
