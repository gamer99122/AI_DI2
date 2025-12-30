// Basic37: API 版本控制
using Microsoft.AspNetCore.Mvc;

namespace Basic37_ApiVersioning;

// 服務介面
public interface IProductService
{
    string GetProductInfo(int id);
}

// V1 服務
public class ProductServiceV1 : IProductService
{
    public string GetProductInfo(int id)
    {
        return $"V1 - 產品 {id}";
    }
}

// V2 服務
public class ProductServiceV2 : IProductService
{
    public string GetProductInfo(int id)
    {
        return $"V2 - 產品 {id} (增強版)";
    }
}

// V1 Controller
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductServiceV1 _productService;

    public ProductsController(ProductServiceV1 productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var info = _productService.GetProductInfo(id);
        return Ok(new { Version = "1.0", Data = info });
    }
}

// V2 Controller
[ApiController]
[Route("api/v2/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    private readonly ProductServiceV2 _productService;

    public ProductsV2Controller(ProductServiceV2 productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var info = _productService.GetProductInfo(id);
        return Ok(new { Version = "2.0", Data = info, Enhanced = true });
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic37: API 版本控制 ===\n");

        var builder = WebApplication.CreateBuilder(args);

        // 註冊服務
        builder.Services.AddControllers();
        builder.Services.AddScoped<ProductServiceV1>();
        builder.Services.AddScoped<ProductServiceV2>();

        var app = builder.Build();

        app.MapControllers();

        Console.WriteLine("API 啟動！");
        Console.WriteLine("端點：");
        Console.WriteLine("  V1: GET /api/v1/products/{id}");
        Console.WriteLine("  V2: GET /api/v2/products/{id}");
        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 不同版本使用不同的服務實作");
        Console.WriteLine("✅ 透過 URL 路徑區分版本");
        Console.WriteLine("✅ 可以逐步遷移用戶到新版本");

        app.Run();
    }
}
