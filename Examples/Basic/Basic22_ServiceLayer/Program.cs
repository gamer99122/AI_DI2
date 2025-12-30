// Basic22: Service 層實作
using Microsoft.Extensions.DependencyInjection;

namespace Basic22_ServiceLayer;

// ===== Model 層 =====
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// ===== Repository 層（資料存取）=====
public interface IProductRepository
{
    Product? GetById(int id);
    void Update(Product product);
}

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "筆記型電腦", Price = 30000, Stock = 10 },
        new Product { Id = 2, Name = "滑鼠", Price = 500, Stock = 50 }
    };

    public Product? GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        Console.WriteLine($"[Repository] 取得產品: {product?.Name ?? "not found"}");
        return product;
    }

    public void Update(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
        {
            existing.Stock = product.Stock;
            Console.WriteLine($"[Repository] 更新產品庫存: {product.Name} = {product.Stock}");
        }
    }
}

// ===== Service 層（業務邏輯）=====
public interface IProductService
{
    bool PurchaseProduct(int productId, int quantity);
    Product? GetProduct(int productId);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Product? GetProduct(int productId)
    {
        Console.WriteLine($"[Service] 查詢產品 ID: {productId}");
        return _repository.GetById(productId);
    }

    public bool PurchaseProduct(int productId, int quantity)
    {
        Console.WriteLine($"\n[Service] 處理購買請求: ProductId={productId}, Quantity={quantity}");

        // 業務邏輯：檢查庫存
        var product = _repository.GetById(productId);

        if (product == null)
        {
            Console.WriteLine("[Service] ❌ 產品不存在");
            return false;
        }

        if (product.Stock < quantity)
        {
            Console.WriteLine($"[Service] ❌ 庫存不足（需要:{quantity}, 現有:{product.Stock}）");
            return false;
        }

        // 業務邏輯：計算價格
        var totalPrice = product.Price * quantity;
        Console.WriteLine($"[Service] 總價: ${totalPrice}");

        // 業務邏輯：扣除庫存
        product.Stock -= quantity;
        _repository.Update(product);

        Console.WriteLine($"[Service] ✅ 購買成功！剩餘庫存: {product.Stock}");
        return true;
    }
}

// ===== Controller/UI 層（展示層）=====
public class ProductController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public void ShowProduct(int productId)
    {
        var product = _productService.GetProduct(productId);
        if (product != null)
        {
            Console.WriteLine($"\n產品資訊:");
            Console.WriteLine($"  名稱: {product.Name}");
            Console.WriteLine($"  價格: ${product.Price}");
            Console.WriteLine($"  庫存: {product.Stock}");
        }
    }

    public void BuyProduct(int productId, int quantity)
    {
        var success = _productService.PurchaseProduct(productId, quantity);
        Console.WriteLine(success ? "\n購買流程完成！" : "\n購買失敗！");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic22: Service 層實作 ===\n");

        var services = new ServiceCollection();

        // 註冊各層服務
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ProductController>();

        var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var controller = scope.ServiceProvider.GetRequiredService<ProductController>();

            Console.WriteLine("=== 場景 1: 查看產品 ===");
            controller.ShowProduct(1);

            Console.WriteLine("\n=== 場景 2: 成功購買 ===");
            controller.BuyProduct(1, 2);

            controller.ShowProduct(1);

            Console.WriteLine("\n=== 場景 3: 庫存不足 ===");
            controller.BuyProduct(1, 100);

            Console.WriteLine("\n=== 場景 4: 產品不存在 ===");
            controller.BuyProduct(999, 1);
        }

        Console.WriteLine("\n\n架構說明：");
        Console.WriteLine("✅ Repository 層：負責資料存取");
        Console.WriteLine("✅ Service 層：負責業務邏輯");
        Console.WriteLine("✅ Controller 層：負責協調和展示");
        Console.WriteLine("✅ 每層職責分明，易於測試和維護");
    }
}
