// Basic31: Controller 中使用 DI
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Basic31_ControllerDI;

// 服務介面和實作
public interface IWeatherService
{
    string GetWeather(string city);
}

public class WeatherService : IWeatherService
{
    public string GetWeather(string city)
    {
        return $"{city} 的天氣：晴天，溫度 25°C";
    }
}

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

// API Controller
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger _logger;

    // ✅ 透過建構函數注入服務
    public WeatherController(IWeatherService weatherService, ILogger logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("{city}")]
    public IActionResult Get(string city)
    {
        _logger.Log($"取得城市天氣: {city}");

        var weather = _weatherService.GetWeather(city);

        return Ok(new { City = city, Weather = weather });
    }

    [HttpGet("cities")]
    public IActionResult GetCities()
    {
        var cities = new[] { "台北", "台中", "高雄" };
        return Ok(cities);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic31: Controller DI ===\n");

        var builder = WebApplication.CreateBuilder(args);

        // ✅ 註冊服務
        builder.Services.AddControllers();
        builder.Services.AddScoped<IWeatherService, WeatherService>();
        builder.Services.AddScoped<ILogger, ConsoleLogger>();

        var app = builder.Build();

        app.MapControllers();

        Console.WriteLine("API 啟動成功！");
        Console.WriteLine("可用端點：");
        Console.WriteLine("  GET /api/weather/{city}");
        Console.WriteLine("  GET /api/weather/cities");
        Console.WriteLine("\n按 Ctrl+C 停止\n");

        app.Run();
    }
}
