// ========================================
// Basic01: 沒有使用 DI 的程式碼
// ========================================
// 目的：展示傳統方式的問題
// - 緊耦合
// - 難以測試
// - 難以維護

namespace Basic01_WithoutDI;

/// <summary>
/// 電子郵件服務（具體實作）
/// </summary>
public class EmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"[EmailService] 發送郵件到: {to}");
        Console.WriteLine($"主旨: {subject}");
        Console.WriteLine($"內容: {body}");
        Console.WriteLine();
    }
}

/// <summary>
/// 用戶服務（直接創建依賴）
/// </summary>
public class UserService
{
    // ❌ 問題 1：直接 new EmailService，造成緊耦合
    private readonly EmailService _emailService = new EmailService();

    public void RegisterUser(string username, string email)
    {
        Console.WriteLine($"[UserService] 註冊用戶: {username}");

        // 模擬保存到資料庫
        Console.WriteLine($"[UserService] 將用戶資料存入資料庫...");

        // ❌ 問題 2：無法替換為其他通知方式（SMS, Push）
        // ❌ 問題 3：單元測試時無法 Mock EmailService
        _emailService.SendEmail(email, "歡迎註冊", $"親愛的 {username}，歡迎加入！");

        Console.WriteLine($"[UserService] 用戶 {username} 註冊成功！");
    }
}

/// <summary>
/// 訂單服務（也是直接創建依賴）
/// </summary>
public class OrderService
{
    // ❌ 問題：每個需要發送郵件的服務都要 new EmailService
    private readonly EmailService _emailService = new EmailService();

    public void CreateOrder(int orderId, string customerEmail, decimal amount)
    {
        Console.WriteLine($"[OrderService] 建立訂單: #{orderId}");
        Console.WriteLine($"[OrderService] 金額: ${amount}");

        _emailService.SendEmail(
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
        Console.WriteLine("Basic01: 沒有使用 DI 的程式碼範例");
        Console.WriteLine("========================================\n");

        // ❌ 主程式直接創建服務
        var userService = new UserService();
        var orderService = new OrderService();

        Console.WriteLine("--- 測試 1: 註冊用戶 ---");
        userService.RegisterUser("張三", "zhangsan@example.com");

        Console.WriteLine("\n--- 測試 2: 建立訂單 ---");
        orderService.CreateOrder(1001, "lisi@example.com", 299.99m);

        Console.WriteLine("\n========================================");
        Console.WriteLine("這個範例的問題：");
        Console.WriteLine("1. UserService 和 OrderService 都直接 new EmailService");
        Console.WriteLine("2. 無法替換為 SMS 或其他通知方式");
        Console.WriteLine("3. 無法在測試時使用 Mock 物件");
        Console.WriteLine("4. EmailService 的變更會影響所有使用它的類別");
        Console.WriteLine("5. 違反依賴反轉原則（DIP）");
        Console.WriteLine("========================================");
    }
}
