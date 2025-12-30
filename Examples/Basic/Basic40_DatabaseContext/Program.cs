// Basic40: EF Core DbContext
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Basic40_DatabaseContext;

// 實體類別
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

// DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 設定初始資料
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "筆記型電腦", Price = 30000 },
            new Product { Id = 2, Name = "滑鼠", Price = 500 },
            new Product { Id = 3, Name = "鍵盤", Price = 1500 }
        );
    }
}

// Repository
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
}

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    // ✅ 注入 DbContext（Scoped）
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        Console.WriteLine("[Repository] 取得所有產品");
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        Console.WriteLine($"[Repository] 取得產品 ID: {id}");
        return await _context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        Console.WriteLine($"[Repository] 新增產品: {product.Name}");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}

// Service
public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ShowAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();

        Console.WriteLine($"\n所有產品 ({products.Count} 筆):");
        foreach (var product in products)
        {
            Console.WriteLine($"  #{product.Id} {product.Name} - ${product.Price}");
        }
    }

    public async Task AddProductAsync(string name, decimal price)
    {
        var product = new Product { Name = name, Price = price };
        await _repository.AddAsync(product);
        Console.WriteLine($"✅ 產品已新增");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Basic40: EF Core DbContext ===\n");

        var services = new ServiceCollection();

        // ✅ 註冊 DbContext（使用 In-Memory 資料庫）
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("DemoDb");
            options.EnableSensitiveDataLogging(); // 開發環境使用
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ProductService>();

        var provider = services.BuildServiceProvider();

        // 初始化資料庫
        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("[DbContext] 資料庫已初始化\n");
        }

        // 使用服務
        using (var scope = provider.CreateScope())
        {
            var productService = scope.ServiceProvider.GetRequiredService<ProductService>();

            Console.WriteLine("--- 查詢現有產品 ---");
            await productService.ShowAllProductsAsync();

            Console.WriteLine("\n--- 新增產品 ---");
            await productService.AddProductAsync("耳機", 2000);

            Console.WriteLine("\n--- 再次查詢 ---");
            await productService.ShowAllProductsAsync();
        }

        Console.WriteLine("\n\n重點：");
        Console.WriteLine("✅ DbContext 必須註冊為 Scoped");
        Console.WriteLine("✅ 使用 AddDbContext 註冊");
        Console.WriteLine("✅ 每個 HTTP 請求一個 DbContext 實例");
        Console.WriteLine("✅ Repository 層注入 DbContext");
        Console.WriteLine("✅ 開發環境可使用 In-Memory 資料庫");
        Console.WriteLine("\n生產環境使用：");
        Console.WriteLine("  options.UseSqlServer(connectionString)");
        Console.WriteLine("  options.UseNpgsql(connectionString)");
        Console.WriteLine("  options.UseMySql(connectionString)");
    }
}
