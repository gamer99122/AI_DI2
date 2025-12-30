// Basic35: Razor Pages 中使用 DI
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Basic35_RazorPagesDI;

// 服務
public interface IGreetingService
{
    string GetGreeting(string name);
}

public class GreetingService : IGreetingService
{
    public string GetGreeting(string name)
    {
        var hour = DateTime.Now.Hour;
        var timeGreeting = hour < 12 ? "早安" : hour < 18 ? "午安" : "晚安";
        return $"{timeGreeting}, {name}!";
    }
}

// Razor Page Model
public class IndexModel : PageModel
{
    private readonly IGreetingService _greetingService;

    // ✅ 透過建構函數注入
    public IndexModel(IGreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    public string Greeting { get; set; } = "";

    public void OnGet(string name = "訪客")
    {
        Greeting = _greetingService.GetGreeting(name);
        Console.WriteLine($"[Razor Page] 生成問候語: {Greeting}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic35: Razor Pages DI ===\n");

        var builder = WebApplication.CreateBuilder(args);

        // 註冊服務
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<IGreetingService, GreetingService>();

        var app = builder.Build();

        app.MapRazorPages();

        // 簡單的測試端點
        app.MapGet("/test", (IGreetingService service) =>
        {
            return service.GetGreeting("測試用戶");
        });

        Console.WriteLine("Razor Pages 應用程式啟動！");
        Console.WriteLine("  GET /test");
        Console.WriteLine("\n在 Razor Pages 中：");
        Console.WriteLine("✅ 在 PageModel 中注入服務");
        Console.WriteLine("✅ 在 .cshtml 中使用 @inject");

        app.Run();
    }
}
