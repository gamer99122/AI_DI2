// ========================================
// Basic02: 使用 DI 改善 Basic01
// ========================================
// 目的：展示如何使用依賴注入解決問題
// - 使用介面抽象化依賴
// - 透過建構函數注入
// - 使用 ServiceCollection 管理依賴

using Microsoft.Extensions.DependencyInjection;

namespace Basic02_WithDI;

/// <summary>
/// 通知服務介面（抽象）
/// </summary>
public interface INotificationService
{
    void Send(string to, string subject, string message);
}

/// <summary>
/// 電子郵件服務（實作）
/// </summary>
public class EmailService : INotificationService
{
    public void Send(string to, string subject, string message)
    {
        Console.WriteLine($"[EmailService] 發送郵件到: {to}");
        Console.WriteLine($"主旨: {subject}");
        Console.WriteLine($"內容: {message}");
        Console.WriteLine();
    }
}

/// <summary>
/// 用戶服務（透過建構函數注入依賴）
/// </summary>
public class UserService
{
    private readonly INotificationService _notificationService;

    // ✅ 解決方案 1：透過建構函數注入介面
    public UserService(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void RegisterUser(string username, string email)
    {
        Console.WriteLine($"[UserService] 註冊用戶: {username}");
        Console.WriteLine($"[UserService] 將用戶資料存入資料庫...");

        // ✅ 使用注入的服務，而非直接 new
        _notificationService.Send(email, "歡迎註冊", $"親愛的 {username}，歡迎加入！");

        Console.WriteLine($"[UserService] 用戶 {username} 註冊成功！");
    }
}

/// <summary>
/// 訂單服務（同樣透過建構函數注入）
/// </summary>
public class OrderService
{
    private readonly INotificationService _notificationService;

    public OrderService(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void CreateOrder(int orderId, string customerEmail, decimal amount)
    {
        Console.WriteLine($"[OrderService] 建立訂單: #{orderId}");
        Console.WriteLine($"[OrderService] 金額: ${amount}");

        _notificationService.Send(
            customerEmail,
            "訂單確認",
            $"您的訂單 #{orderId} 已成功建立，金額: ${amount}"
        );

        Console.WriteLine($"[OrderService] 訂單 #{orderId} 建立成功！");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Basic02: 使用 DI 的程式碼範例");
        Console.WriteLine("========================================\n");

        // ✅ 步驟 1：建立服務容器
        var services = new ServiceCollection();

        // ✅ 步驟 2：註冊服務
        // 將 INotificationService 映射到 EmailService
        services.AddTransient<INotificationService, EmailService>();

        // 註冊 UserService 和 OrderService
        services.AddTransient<UserService>();
        services.AddTransient<OrderService>();

        // ✅ 步驟 3：建立 ServiceProvider
        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--- 測試 1: 註冊用戶 ---");
        // ✅ 步驟 4：從容器解析服務
        var userService = serviceProvider.GetRequiredService<UserService>();
        userService.RegisterUser("張三", "zhangsan@example.com");

        Console.WriteLine("\n--- 測試 2: 建立訂單 ---");
        var orderService = serviceProvider.GetRequiredService<OrderService>();
        orderService.CreateOrder(1001, "lisi@example.com", 299.99m);

        Console.WriteLine("\n========================================");
        Console.WriteLine("使用 DI 的優點：");
        Console.WriteLine("✅ 1. 鬆耦合：UserService 不知道 EmailService 的存在");
        Console.WriteLine("✅ 2. 易擴展：可以輕鬆替換為 SmsService");
        Console.WriteLine("✅ 3. 易測試：可以注入 Mock 物件");
        Console.WriteLine("✅ 4. 集中管理：所有依賴在一個地方註冊");
        Console.WriteLine("✅ 5. 符合 SOLID 原則");
        Console.WriteLine("========================================");
    }
}
