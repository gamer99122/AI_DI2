// Basic25: 工廠模式
using Microsoft.Extensions.DependencyInjection;

namespace Basic25_FactoryPattern;

public enum NotificationType { Email, Sms, Push }

public interface INotification
{
    void Send(string message);
}

public class EmailNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"[Email] {message}");
}

public class SmsNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"[SMS] {message}");
}

public class PushNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"[Push] {message}");
}

// 工廠介面
public interface INotificationFactory
{
    INotification Create(NotificationType type);
}

// 工廠實作
public class NotificationFactory : INotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotification Create(NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => _serviceProvider.GetRequiredService<EmailNotification>(),
            NotificationType.Sms => _serviceProvider.GetRequiredService<SmsNotification>(),
            NotificationType.Push => _serviceProvider.GetRequiredService<PushNotification>(),
            _ => throw new ArgumentException($"Unknown type: {type}")
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic25: 工廠模式 ===\n");

        var services = new ServiceCollection();

        // 註冊所有實作
        services.AddTransient<EmailNotification>();
        services.AddTransient<SmsNotification>();
        services.AddTransient<PushNotification>();

        // 註冊工廠
        services.AddSingleton<INotificationFactory, NotificationFactory>();

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<INotificationFactory>();

        // 動態建立不同類型的通知
        var email = factory.Create(NotificationType.Email);
        email.Send("Email 通知");

        var sms = factory.Create(NotificationType.Sms);
        sms.Send("SMS 通知");

        var push = factory.Create(NotificationType.Push);
        push.Send("Push 通知");

        Console.WriteLine("\n✅ 工廠模式用於動態建立物件");
        Console.WriteLine("✅ 可以在執行時決定建立哪種實作");
    }
}
