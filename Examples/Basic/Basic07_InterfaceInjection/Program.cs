// Basic07: 介面注入的重要性
using Microsoft.Extensions.DependencyInjection;

namespace Basic07_InterfaceInjection;

// 支付服務介面
public interface IPaymentService
{
    bool ProcessPayment(decimal amount);
}

// 實作1：信用卡支付
public class CreditCardPayment : IPaymentService
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[信用卡] 處理支付: ${amount}");
        return true;
    }
}

// 實作2：PayPal支付
public class PayPalPayment : IPaymentService
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[PayPal] 處理支付: ${amount}");
        return true;
    }
}

// 訂單服務依賴介面
public class OrderService
{
    private readonly IPaymentService _paymentService;

    public OrderService(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public void PlaceOrder(int orderId, decimal amount)
    {
        Console.WriteLine($"處理訂單 #{orderId}...");
        _paymentService.ProcessPayment(amount);
        Console.WriteLine("訂單完成！\n");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic07: 介面注入 ===\n");

        Console.WriteLine("--- 場景1：使用信用卡 ---");
        var services1 = new ServiceCollection();
        services1.AddTransient<IPaymentService, CreditCardPayment>();
        services1.AddTransient<OrderService>();
        var provider1 = services1.BuildServiceProvider();

        var orderService1 = provider1.GetRequiredService<OrderService>();
        orderService1.PlaceOrder(1001, 99.99m);

        Console.WriteLine("--- 場景2：改用 PayPal ---");
        var services2 = new ServiceCollection();
        services2.AddTransient<IPaymentService, PayPalPayment>(); // ← 只改這裡！
        services2.AddTransient<OrderService>();
        var provider2 = services2.BuildServiceProvider();

        var orderService2 = provider2.GetRequiredService<OrderService>();
        orderService2.PlaceOrder(1002, 149.99m);

        Console.WriteLine("重點：");
        Console.WriteLine("✅ OrderService 不需要修改任何程式碼");
        Console.WriteLine("✅ 只需更換註冊就能切換實作");
        Console.WriteLine("✅ 符合開放封閉原則（OCP）");
        Console.WriteLine("✅ 易於單元測試（可注入 Mock）");
    }
}
