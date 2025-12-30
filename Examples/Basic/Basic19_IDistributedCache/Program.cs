// Basic19: IDistributedCache - 分散式快取
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Basic19_IDistributedCache;

/// <summary>
/// 用戶服務，使用分散式快取
/// </summary>
public class UserService
{
    private readonly IDistributedCache _cache;

    public UserService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetUserAsync(string userId)
    {
        var cacheKey = $"user:{userId}";

        // 嘗試從快取取得
        var cachedValue = await _cache.GetStringAsync(cacheKey);

        if (cachedValue != null)
        {
            Console.WriteLine($"[Cache Hit] 從快取取得用戶: {cachedValue}");
            return cachedValue;
        }

        // 快取未命中，從資料庫載入（模擬）
        Console.WriteLine($"[Cache Miss] 從資料庫載入用戶: {userId}");
        var userName = $"User_{userId}";

        // 存入快取，設定過期時間
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(cacheKey, userName, options);
        Console.WriteLine($"[Cache Set] 已快取用戶資料");

        return userName;
    }

    public async Task RemoveUserAsync(string userId)
    {
        var cacheKey = $"user:{userId}";
        await _cache.RemoveAsync(cacheKey);
        Console.WriteLine($"[Cache Remove] 已移除用戶快取: {userId}");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Basic19: IDistributedCache ===\n");

        var services = new ServiceCollection();

        // 註冊記憶體分散式快取（開發環境使用）
        // 生產環境可改用 Redis 或 SQL Server
        services.AddDistributedMemoryCache();

        services.AddTransient<UserService>();

        var provider = services.BuildServiceProvider();
        var userService = provider.GetRequiredService<UserService>();

        Console.WriteLine("--- 第一次存取 ---");
        await userService.GetUserAsync("001");

        Console.WriteLine("\n--- 第二次存取（從快取） ---");
        await userService.GetUserAsync("001");

        Console.WriteLine("\n--- 移除快取 ---");
        await userService.RemoveUserAsync("001");

        Console.WriteLine("\n--- 第三次存取（快取已清除）---");
        await userService.GetUserAsync("001");

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ IDistributedCache 支援分散式快取");
        Console.WriteLine("✅ 可以用 Redis、SQL Server 等實作");
        Console.WriteLine("✅ 適用於多伺服器環境");
        Console.WriteLine("✅ 支援過期時間設定");
    }
}
