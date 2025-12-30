// Basic33: Middleware 中使用 DI
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Basic33_MiddlewareDI;

// 服務
public interface IRequestCounter
{
    int Increment();
    int GetCount();
}

public class RequestCounter : IRequestCounter
{
    private int _count = 0;

    public int Increment()
    {
        return Interlocked.Increment(ref _count);
    }

    public int GetCount() => _count;
}

// Middleware
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    // ⚠️ 建構函數只能注入 Singleton 服務

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // ✅ InvokeAsync 可以注入 Scoped 服務
    public async Task InvokeAsync(HttpContext context, IRequestCounter counter)
    {
        var requestNumber = counter.Increment();

        Console.WriteLine($"[Middleware] 請求 #{requestNumber}");
        Console.WriteLine($"  路徑: {context.Request.Path}");
        Console.WriteLine($"  方法: {context.Request.Method}");

        await _next(context);

        Console.WriteLine($"[Middleware] 請求 #{requestNumber} 完成\n");
    }
}

// 擴充方法
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic33: Middleware DI ===\n");

        var builder = WebApplication.CreateBuilder(args);

        // 註冊服務
        builder.Services.AddSingleton<IRequestCounter, RequestCounter>();

        var app = builder.Build();

        // ✅ 使用自訂 Middleware
        app.UseRequestLogging();

        app.MapGet("/", () => "Hello from Middleware Demo!");

        app.MapGet("/test", (IRequestCounter counter) =>
        {
            return new { TotalRequests = counter.GetCount() };
        });

        Console.WriteLine("API 啟動！");
        Console.WriteLine("端點：");
        Console.WriteLine("  GET /");
        Console.WriteLine("  GET /test");
        Console.WriteLine();

        app.Run();
    }
}
