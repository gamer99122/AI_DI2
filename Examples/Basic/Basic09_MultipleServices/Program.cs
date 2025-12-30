// Basic09: 註冊多個服務
using Microsoft.Extensions.DependencyInjection;

namespace Basic09_MultipleServices;

public interface IEmailService { void Send(string msg); }
public interface ISmsService { void Send(string msg); }
public interface IPushService { void Send(string msg); }

public class EmailService : IEmailService
{
    public void Send(string msg) => Console.WriteLine($"[Email] {msg}");
}

public class SmsService : ISmsService
{
    public void Send(string msg) => Console.WriteLine($"[SMS] {msg}");
}

public class PushService : IPushService
{
    public void Send(string msg) => Console.WriteLine($"[Push] {msg}");
}

public class NotificationManager
{
    private readonly IEmailService _email;
    private readonly ISmsService _sms;
    private readonly IPushService _push;

    public NotificationManager(IEmailService email, ISmsService sms, IPushService push)
    {
        _email = email;
        _sms = sms;
        _push = push;
    }

    public void NotifyAll(string message)
    {
        _email.Send(message);
        _sms.Send(message);
        _push.Send(message);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic09: 註冊多個服務 ===\n");

        var services = new ServiceCollection();

        // 註冊多個服務
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ISmsService, SmsService>();
        services.AddTransient<IPushService, PushService>();
        services.AddTransient<NotificationManager>();

        var provider = services.BuildServiceProvider();
        var manager = provider.GetRequiredService<NotificationManager>();

        manager.NotifyAll("系統維護通知");

        Console.WriteLine("\n✅ DI 容器自動解析所有依賴");
    }
}
