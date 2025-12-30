// Basic34: Filter 中使用 DI
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Basic34_FilterDI;

// 日誌服務
public interface IActionLogger
{
    void LogAction(string actionName, string controllerName);
}

public class ActionLogger : IActionLogger
{
    public void LogAction(string actionName, string controllerName)
    {
        Console.WriteLine($"[ActionLogger] {controllerName}.{actionName} 被呼叫");
    }
}

// Action Filter
public class LoggingActionFilter : IActionFilter
{
    private readonly IActionLogger _logger;

    // ✅ 透過建構函數注入
    public LoggingActionFilter(IActionLogger logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.RouteValues["action"];
        var controllerName = context.ActionDescriptor.RouteValues["controller"];

        _logger.LogAction(actionName ?? "", controllerName ?? "");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"[Filter] Action 執行完成");
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LoggingActionFilter))] // ✅ 使用 ServiceFilter
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Hello from Test API" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, Name = $"Item {id}" });
    }
}

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 註冊服務
        builder.Services.AddControllers();
        builder.Services.AddScoped<IActionLogger, ActionLogger>();
        builder.Services.AddScoped<LoggingActionFilter>(); // ✅ 註冊 Filter

        var app = builder.Build();

        app.MapControllers();

        Console.WriteLine("=== Basic34: Filter DI ===");
        Console.WriteLine("API 啟動！端點：");
        Console.WriteLine("  GET /api/test");
        Console.WriteLine("  GET /api/test/{id}");

        app.Run();
    }
}
