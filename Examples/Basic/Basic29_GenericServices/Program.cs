// Basic29: 泛型服務
using Microsoft.Extensions.DependencyInjection;

namespace Basic29_GenericServices;

// 實體類別
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

// 泛型 Repository 介面
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
}

// 泛型 Repository 實作
public class Repository<T> : IRepository<T> where T : class
{
    private readonly List<T> _data = new();

    public void Add(T entity)
    {
        _data.Add(entity);
        Console.WriteLine($"[Repository<{typeof(T).Name}>] 新增實體");
    }

    public IEnumerable<T> GetAll()
    {
        Console.WriteLine($"[Repository<{typeof(T).Name}>] 取得所有實體 ({_data.Count} 筆)");
        return _data;
    }

    public T? GetById(int id)
    {
        Console.WriteLine($"[Repository<{typeof(T).Name}>] 取得實體 ID: {id}");
        return _data.ElementAtOrDefault(id - 1);
    }
}

// 泛型 Service
public interface IDataService<T> where T : class
{
    void Process(T data);
}

public class DataService<T> : IDataService<T> where T : class
{
    private readonly IRepository<T> _repository;

    public DataService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public void Process(T data)
    {
        Console.WriteLine($"[DataService<{typeof(T).Name}>] 處理資料");
        _repository.Add(data);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic29: 泛型服務 ===\n");

        var services = new ServiceCollection();

        // ✅ 註冊泛型服務
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IDataService<>), typeof(DataService<>));

        var provider = services.BuildServiceProvider();

        Console.WriteLine("--- 使用 User Repository ---");
        using (var scope = provider.CreateScope())
        {
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            userRepo.Add(new User { Id = 1, Name = "張三" });
            userRepo.Add(new User { Id = 2, Name = "李四" });

            var users = userRepo.GetAll();
            foreach (var user in users)
            {
                Console.WriteLine($"  User: {user.Name}");
            }
        }

        Console.WriteLine("\n--- 使用 Product Repository ---");
        using (var scope = provider.CreateScope())
        {
            var productRepo = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

            productRepo.Add(new Product { Id = 1, Name = "筆記型電腦" });
            productRepo.Add(new Product { Id = 2, Name = "滑鼠" });

            var products = productRepo.GetAll();
            foreach (var product in products)
            {
                Console.WriteLine($"  Product: {product.Name}");
            }
        }

        Console.WriteLine("\n--- 使用泛型 Service ---");
        using (var scope = provider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IDataService<User>>();
            userService.Process(new User { Id = 3, Name = "王五" });

            var productService = scope.ServiceProvider.GetRequiredService<IDataService<Product>>();
            productService.Process(new Product { Id = 3, Name = "鍵盤" });
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 使用 typeof() 註冊泛型服務");
        Console.WriteLine("✅ 一次註冊，多種類型使用");
        Console.WriteLine("✅ 減少重複程式碼");
        Console.WriteLine("✅ 型別安全");
    }
}
