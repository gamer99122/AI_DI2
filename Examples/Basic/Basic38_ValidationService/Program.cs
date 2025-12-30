// Basic38: 驗證服務
using Microsoft.Extensions.DependencyInjection;

namespace Basic38_ValidationService;

// 驗證結果
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

// 驗證器介面
public interface IValidator<T>
{
    ValidationResult Validate(T item);
}

// 用戶模型
public class User
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; }
}

// 用戶驗證器
public class UserValidator : IValidator<User>
{
    public ValidationResult Validate(User user)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(user.Name))
        {
            result.IsValid = false;
            result.Errors.Add("姓名不可為空");
        }

        if (!user.Email.Contains("@"))
        {
            result.IsValid = false;
            result.Errors.Add("電子郵件格式不正確");
        }

        if (user.Age < 0 || user.Age > 150)
        {
            result.IsValid = false;
            result.Errors.Add("年齡必須在 0-150 之間");
        }

        return result;
    }
}

// 用戶服務
public class UserService
{
    private readonly IValidator<User> _validator;

    public UserService(IValidator<User> validator)
    {
        _validator = validator;
    }

    public bool CreateUser(User user)
    {
        Console.WriteLine($"\n嘗試建立用戶: {user.Name}");

        var validationResult = _validator.Validate(user);

        if (!validationResult.IsValid)
        {
            Console.WriteLine("❌ 驗證失敗：");
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"  - {error}");
            }
            return false;
        }

        Console.WriteLine("✅ 驗證通過，用戶建立成功");
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic38: 驗證服務 ===\n");

        var services = new ServiceCollection();

        services.AddScoped<IValidator<User>, UserValidator>();
        services.AddScoped<UserService>();

        var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();

            // 測試 1: 有效用戶
            Console.WriteLine("--- 測試 1: 有效用戶 ---");
            var validUser = new User
            {
                Name = "張三",
                Email = "zhangsan@example.com",
                Age = 25
            };
            userService.CreateUser(validUser);

            // 測試 2: 無效用戶（多個錯誤）
            Console.WriteLine("\n--- 測試 2: 無效用戶 ---");
            var invalidUser = new User
            {
                Name = "",
                Email = "invalid-email",
                Age = -5
            };
            userService.CreateUser(invalidUser);

            // 測試 3: 部分無效
            Console.WriteLine("\n--- 測試 3: 部分無效 ---");
            var partialInvalidUser = new User
            {
                Name = "李四",
                Email = "lisi",
                Age = 30
            };
            userService.CreateUser(partialInvalidUser);
        }

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 將驗證邏輯獨立為服務");
        Console.WriteLine("✅ 易於單元測試");
        Console.WriteLine("✅ 可重複使用");
        Console.WriteLine("✅ 符合單一職責原則");
    }
}
