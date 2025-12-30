// Basic11: ILogger 注入
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basic11_ILoggerInjection;

public class UserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public void CreateUser(string name)
    {
        _logger.LogInformation("開始建立用戶: {Name}", name);
        // 模擬工作
        _logger.LogDebug("驗證用戶資料...");
        _logger.LogWarning("用戶 {Name} 未設定頭像", name);
        _logger.LogInformation("用戶建立成功");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic11: ILogger 注入 ===\n");

        var services = new ServiceCollection();

        // 添加日誌服務
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddTransient<UserService>();

        var provider = services.BuildServiceProvider();
        var userService = provider.GetRequiredService<UserService>();

        userService.CreateUser("張三");

        Console.WriteLine("\n✅ ILogger<T> 會自動注入");
        Console.WriteLine("✅ T 是使用日誌的類別名稱");
    }
}
