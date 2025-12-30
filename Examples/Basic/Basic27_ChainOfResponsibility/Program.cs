// Basic27: 責任鏈模式
using Microsoft.Extensions.DependencyInjection;

namespace Basic27_ChainOfResponsibility;

// 請求類別
public class Request
{
    public string Type { get; set; } = "";
    public string Content { get; set; } = "";
    public bool Handled { get; set; }
}

// 處理器介面
public interface IHandler
{
    void SetNext(IHandler handler);
    void Handle(Request request);
}

// 抽象處理器
public abstract class BaseHandler : IHandler
{
    private IHandler? _next;

    public void SetNext(IHandler handler)
    {
        _next = handler;
    }

    public virtual void Handle(Request request)
    {
        if (_next != null && !request.Handled)
        {
            _next.Handle(request);
        }
    }
}

// 具體處理器 1
public class EmailHandler : BaseHandler
{
    public override void Handle(Request request)
    {
        if (request.Type == "email")
        {
            Console.WriteLine($"[EmailHandler] 處理郵件: {request.Content}");
            request.Handled = true;
        }
        else
        {
            base.Handle(request);
        }
    }
}

// 具體處理器 2
public class SmsHandler : BaseHandler
{
    public override void Handle(Request request)
    {
        if (request.Type == "sms")
        {
            Console.WriteLine($"[SmsHandler] 處理簡訊: {request.Content}");
            request.Handled = true;
        }
        else
        {
            base.Handle(request);
        }
    }
}

// 具體處理器 3
public class PushHandler : BaseHandler
{
    public override void Handle(Request request)
    {
        if (request.Type == "push")
        {
            Console.WriteLine($"[PushHandler] 處理推播: {request.Content}");
            request.Handled = true;
        }
        else
        {
            base.Handle(request);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic27: 責任鏈模式 ===\n");

        var services = new ServiceCollection();
        services.AddTransient<EmailHandler>();
        services.AddTransient<SmsHandler>();
        services.AddTransient<PushHandler>();

        var provider = services.BuildServiceProvider();

        // 建立責任鏈
        var emailHandler = provider.GetRequiredService<EmailHandler>();
        var smsHandler = provider.GetRequiredService<SmsHandler>();
        var pushHandler = provider.GetRequiredService<PushHandler>();

        emailHandler.SetNext(smsHandler);
        smsHandler.SetNext(pushHandler);

        // 測試不同類型的請求
        var requests = new[]
        {
            new Request { Type = "email", Content = "測試郵件" },
            new Request { Type = "sms", Content = "測試簡訊" },
            new Request { Type = "push", Content = "測試推播" },
            new Request { Type = "unknown", Content = "未知類型" }
        };

        foreach (var request in requests)
        {
            Console.WriteLine($"處理請求: {request.Type}");
            emailHandler.Handle(request);

            if (!request.Handled)
            {
                Console.WriteLine("  ❌ 沒有處理器可以處理此請求");
            }
            Console.WriteLine();
        }

        Console.WriteLine("重點：");
        Console.WriteLine("✅ 請求沿著鏈傳遞");
        Console.WriteLine("✅ 每個處理器決定是否處理");
        Console.WriteLine("✅ 可以動態組合處理器");
    }
}
