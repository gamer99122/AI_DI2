// Basic30: 開放泛型類型
using Microsoft.Extensions.DependencyInjection;

namespace Basic30_OpenGenericTypes;

// 泛型驗證器介面
public interface IValidator<T>
{
    bool Validate(T item);
}

// 用戶驗證器
public class UserValidator : IValidator<User>
{
    public bool Validate(User user)
    {
        Console.WriteLine($"[UserValidator] 驗證用戶: {user.Name}");
        var isValid = !string.IsNullOrEmpty(user.Name) && user.Age >= 0;
        Console.WriteLine($"  結果: {(isValid ? "✅ 通過" : "❌ 失敗")}");
        return isValid;
    }
}

// 產品驗證器
public class ProductValidator : IValidator<Product>
{
    public bool Validate(Product product)
    {
        Console.WriteLine($"[ProductValidator] 驗證產品: {product.Name}");
        var isValid = !string.IsNullOrEmpty(product.Name) && product.Price > 0;
        Console.WriteLine($"  結果: {(isValid ? "✅ 通過" : "❌ 失敗")}");
        return isValid;
    }
}

// 實體類別
public class User
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public class Product
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

// 泛型處理器
public class EntityProcessor<T>
{
    private readonly IValidator<T> _validator;

    public EntityProcessor(IValidator<T> validator)
    {
        _validator = validator;
    }

    public void Process(T entity)
    {
        Console.WriteLine($"\n處理 {typeof(T).Name}...");
        if (_validator.Validate(entity))
        {
            Console.WriteLine("✅ 處理成功");
        }
        else
        {
            Console.WriteLine("❌ 驗證失敗，無法處理");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic30: 開放泛型類型 ===\n");

        var services = new ServiceCollection();

        // ✅ 註冊開放泛型類型
        services.AddScoped(typeof(IValidator<>), typeof(UserValidator));  // ❌ 這樣不行

        // ✅ 正確方式：個別註冊每個具體實作
        services.AddScoped<IValidator<User>, UserValidator>();
        services.AddScoped<IValidator<Product>, ProductValidator>();

        // 註冊泛型處理器
        services.AddScoped(typeof(EntityProcessor<>));

        var provider = services.BuildServiceProvider();

        Console.WriteLine("--- 處理用戶 ---");
        using (var scope = provider.CreateScope())
        {
            var userProcessor = scope.ServiceProvider.GetRequiredService<EntityProcessor<User>>();

            var validUser = new User { Name = "張三", Age = 25 };
            userProcessor.Process(validUser);

            var invalidUser = new User { Name = "", Age = -1 };
            userProcessor.Process(invalidUser);
        }

        Console.WriteLine("\n--- 處理產品 ---");
        using (var scope = provider.CreateScope())
        {
            var productProcessor = scope.ServiceProvider.GetRequiredService<EntityProcessor<Product>>();

            var validProduct = new Product { Name = "筆記型電腦", Price = 30000 };
            productProcessor.Process(validProduct);

            var invalidProduct = new Product { Name = "無效產品", Price = -100 };
            productProcessor.Process(invalidProduct);
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 開放泛型類型允許泛型服務");
        Console.WriteLine("✅ 使用 typeof(IService<>) 註冊");
        Console.WriteLine("✅ 自動解析具體類型");
        Console.WriteLine("⚠️  介面實作需個別註冊");
    }
}
