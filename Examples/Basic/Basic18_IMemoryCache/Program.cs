// Basic18: 記憶體快取
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Basic18_IMemoryCache;

public class DataService
{
    private readonly IMemoryCache _cache;
    public DataService(IMemoryCache cache) => _cache = cache;

    public string GetData(string key)
    {
        if (!_cache.TryGetValue(key, out string? value))
        {
            value = $"Data_{key}_{DateTime.Now.Ticks}";
            _cache.Set(key, value, TimeSpan.FromMinutes(5));
            Console.WriteLine($"[Cache Miss] 產生新資料: {value}");
        }
        else
        {
            Console.WriteLine($"[Cache Hit] 從快取取得: {value}");
        }
        return value!;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic18: IMemoryCache ===\n");
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<DataService>();

        var provider = services.BuildServiceProvider();
        var dataService = provider.GetRequiredService<DataService>();

        dataService.GetData("user:1");
        dataService.GetData("user:1"); // 從快取
        Console.WriteLine("\n✅ IMemoryCache 用於記憶體快取");
    }
}
