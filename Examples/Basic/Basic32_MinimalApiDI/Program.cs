// Basic32: Minimal API 中使用 DI
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Basic32_MinimalApiDI;

public interface IGreetingService
{
    string Greet(string name);
}

public class GreetingService : IGreetingService
{
    public string Greet(string name) => $"Hello, {name}!";
}

public interface ITimeService
{
    string GetCurrentTime();
}

public class TimeService : ITimeService
{
    public string GetCurrentTime() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ✅ 註冊服務
        builder.Services.AddScoped<IGreetingService, GreetingService>();
        builder.Services.AddScoped<ITimeService, TimeService>();

        var app = builder.Build();

        // ✅ 方式1：直接注入參數
        app.MapGet("/", () => "Minimal API DI 範例");

        // ✅ 方式2：注入單個服務
        app.MapGet("/greet/{name}", (string name, IGreetingService greeting) =>
        {
            return greeting.Greet(name);
        });

        // ✅ 方式3：注入多個服務
        app.MapGet("/info/{name}", (string name, IGreetingService greeting, ITimeService time) =>
        {
            return new
            {
                Greeting = greeting.Greet(name),
                Time = time.GetCurrentTime()
            };
        });

        // ✅ 方式4：注入 HttpContext
        app.MapGet("/request-info", (HttpContext context) =>
        {
            return new
            {
                Path = context.Request.Path.Value,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.Value
            };
        });

        Console.WriteLine("=== Basic32: Minimal API DI ===");
        Console.WriteLine("API 已啟動，請訪問：");
        Console.WriteLine("  GET /");
        Console.WriteLine("  GET /greet/{name}");
        Console.WriteLine("  GET /info/{name}");
        Console.WriteLine("  GET /request-info");

        app.Run();
    }
}
