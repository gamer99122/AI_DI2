// Basic23: 一個介面多個實作
using Microsoft.Extensions.DependencyInjection;

namespace Basic23_MultipleImplementations;

// 通知服務介面
public interface INotificationService
{
    string Name { get; }
    void Send(string message);
}

// 實作 1: Email
public class EmailNotification : INotificationService
{
    public string Name => "Email";
    public void Send(string message) => Console.WriteLine($"📧 [Email] {message}");
}

// 實作 2: SMS
public class SmsNotification : INotificationService
{
    public string Name => "SMS";
    public void Send(string message) => Console.WriteLine($"📱 [SMS] {message}");
}

// 實作 3: Push
public class PushNotification : INotificationService
{
    public string Name => "Push";
    public void Send(string message) => Console.WriteLine($"🔔 [Push] {message}");
}

// 使用多個實作的服務
public class NotificationManager
{
    private readonly IEnumerable<INotificationService> _notificationServices;

    // ✅ 注入所有實作
    public NotificationManager(IEnumerable<INotificationService> notificationServices)
    {
        _notificationServices = notificationServices;
    }

    public void SendToAll(string message)
    {
        Console.WriteLine($"發送訊息: \"{message}\"\n");

        foreach (var service in _notificationServices)
        {
            service.Send(message);
        }
    }

    public void ShowRegisteredServices()
    {
        Console.WriteLine("已註冊的通知服務:");
        foreach (var service in _notificationServices)
        {
            Console.WriteLine($"  - {service.Name}");
        }
        Console.WriteLine();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic23: 多個實作 ===\n");

        var services = new ServiceCollection();

        // ✅ 註冊同一介面的多個實作
        services.AddTransient<INotificationService, EmailNotification>();
        services.AddTransient<INotificationService, SmsNotification>();
        services.AddTransient<INotificationService, PushNotification>();

        services.AddTransient<NotificationManager>();

        var provider = services.BuildServiceProvider();

        // 測試 1: 使用 IEnumerable 取得所有實作
        Console.WriteLine("--- 方式 1: 注入 IEnumerable<T> ---");
        var manager = provider.GetRequiredService<NotificationManager>();
        manager.ShowRegisteredServices();
        manager.SendToAll("系統維護通知");

        Console.WriteLine("\n--- 方式 2: 直接取得所有服務 ---");
        var allServices = provider.GetServices<INotificationService>();
        Console.WriteLine($"取得 {allServices.Count()} 個服務實例");

        foreach (var service in allServices)
        {
            Console.WriteLine($"  {service.GetType().Name}");
        }

        Console.WriteLine("\n--- 方式 3: 取得單一服務（最後註冊的）---");
        var singleService = provider.GetService<INotificationService>();
        Console.WriteLine($"GetService<T> 回傳: {singleService?.GetType().Name}");
        Console.WriteLine("（回傳最後一個註冊的實作）");

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 可以為同一介面註冊多個實作");
        Console.WriteLine("✅ 使用 IEnumerable<T> 取得所有實作");
        Console.WriteLine("✅ 使用 GetService<T> 取得最後註冊的實作");
        Console.WriteLine("✅ 使用 GetServices<T> 取得所有實作");
    }
}
