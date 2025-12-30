// Basic20: IServiceProvider 使用方式
using Microsoft.Extensions.DependencyInjection;

namespace Basic20_IServiceProvider;

public interface IEmailService { void Send(string message); }
public class EmailService : IEmailService
{
    public void Send(string message) => Console.WriteLine($"[Email] {message}");
}

public interface ISmsService { void Send(string message); }
public class SmsService : ISmsService
{
    public void Send(string message) => Console.WriteLine($"[SMS] {message}");
}

/// <summary>
/// ⚠️ 不推薦：Service Locator 反模式
/// </summary>
public class BadNotificationManager
{
    private readonly IServiceProvider _serviceProvider;

    public BadNotificationManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SendAll(string message)
    {
        // ❌ 隱藏依賴，不清楚需要哪些服務
        var emailService = _serviceProvider.GetService<IEmailService>();
        var smsService = _serviceProvider.GetService<ISmsService>();

        emailService?.Send(message);
        smsService?.Send(message);
    }
}

/// <summary>
/// ✅ 推薦：明確的依賴注入
/// </summary>
public class GoodNotificationManager
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    // ✅ 明確顯示所有依賴
    public GoodNotificationManager(IEmailService emailService, ISmsService smsService)
    {
        _emailService = emailService;
        _smsService = smsService;
    }

    public void SendAll(string message)
    {
        _emailService.Send(message);
        _smsService.Send(message);
    }
}

/// <summary>
/// ✅ 合理使用：工廠模式
/// </summary>
public class NotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // ✅ 動態創建物件時使用 IServiceProvider
    public object CreateService(string type)
    {
        return type switch
        {
            "email" => _serviceProvider.GetRequiredService<IEmailService>(),
            "sms" => _serviceProvider.GetRequiredService<ISmsService>(),
            _ => throw new ArgumentException($"Unknown type: {type}")
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic20: IServiceProvider 使用 ===\n");

        var services = new ServiceCollection();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ISmsService, SmsService>();
        services.AddTransient<BadNotificationManager>();
        services.AddTransient<GoodNotificationManager>();
        services.AddTransient<NotificationFactory>();

        var provider = services.BuildServiceProvider();

        Console.WriteLine("--- ❌ 不推薦：Service Locator 模式 ---");
        var badManager = provider.GetRequiredService<BadNotificationManager>();
        badManager.SendAll("測試訊息");
        Console.WriteLine("問題：隱藏依賴，難以測試\n");

        Console.WriteLine("--- ✅ 推薦：明確依賴注入 ---");
        var goodManager = provider.GetRequiredService<GoodNotificationManager>();
        goodManager.SendAll("測試訊息");
        Console.WriteLine("優點：依賴清楚，易於測試\n");

        Console.WriteLine("--- ✅ 合理使用：工廠模式 ---");
        var factory = provider.GetRequiredService<NotificationFactory>();
        var emailService = factory.CreateService("email") as IEmailService;
        emailService?.Send("工廠創建的服務");

        Console.WriteLine("\n重點：");
        Console.WriteLine("❌ 避免直接注入 IServiceProvider（Service Locator 反模式）");
        Console.WriteLine("✅ 優先使用建構函數注入");
        Console.WriteLine("✅ 只在工廠模式等特殊情況使用 IServiceProvider");
    }
}
