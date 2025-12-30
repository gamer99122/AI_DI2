// Basic24: 具名服務（使用鍵值區分）
using Microsoft.Extensions.DependencyInjection;

namespace Basic24_NamedServices;

// 支付介面
public interface IPaymentGateway
{
    string Name { get; }
    bool ProcessPayment(decimal amount);
}

// 支付實作
public class StripePayment : IPaymentGateway
{
    public string Name => "Stripe";
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[Stripe] 處理支付: ${amount}");
        return true;
    }
}

public class PayPalPayment : IPaymentGateway
{
    public string Name => "PayPal";
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[PayPal] 處理支付: ${amount}");
        return true;
    }
}

public class LinePayPayment : IPaymentGateway
{
    public string Name => "LinePay";
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[LinePay] 處理支付: ${amount}");
        return true;
    }
}

// 具名服務工廠
public interface IPaymentFactory
{
    IPaymentGateway GetPaymentGateway(string name);
    IEnumerable<string> GetAvailableGateways();
}

public class PaymentFactory : IPaymentFactory
{
    private readonly Dictionary<string, IPaymentGateway> _gateways;

    public PaymentFactory(IServiceProvider serviceProvider)
    {
        // 建立具名服務映射
        _gateways = new Dictionary<string, IPaymentGateway>(StringComparer.OrdinalIgnoreCase)
        {
            ["Stripe"] = serviceProvider.GetRequiredService<StripePayment>(),
            ["PayPal"] = serviceProvider.GetRequiredService<PayPalPayment>(),
            ["LinePay"] = serviceProvider.GetRequiredService<LinePayPayment>()
        };
    }

    public IPaymentGateway GetPaymentGateway(string name)
    {
        if (_gateways.TryGetValue(name, out var gateway))
        {
            return gateway;
        }

        throw new ArgumentException($"找不到支付閘道: {name}");
    }

    public IEnumerable<string> GetAvailableGateways()
    {
        return _gateways.Keys;
    }
}

// 訂單服務
public class OrderService
{
    private readonly IPaymentFactory _paymentFactory;

    public OrderService(IPaymentFactory paymentFactory)
    {
        _paymentFactory = paymentFactory;
    }

    public void PlaceOrder(string paymentMethod, decimal amount)
    {
        Console.WriteLine($"\n處理訂單...");
        Console.WriteLine($"  金額: ${amount}");
        Console.WriteLine($"  付款方式: {paymentMethod}");

        try
        {
            var gateway = _paymentFactory.GetPaymentGateway(paymentMethod);
            var success = gateway.ProcessPayment(amount);

            if (success)
            {
                Console.WriteLine("✅ 訂單完成！");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }

    public void ShowAvailablePaymentMethods()
    {
        Console.WriteLine("可用的付款方式:");
        foreach (var gateway in _paymentFactory.GetAvailableGateways())
        {
            Console.WriteLine($"  - {gateway}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic24: 具名服務 ===\n");

        var services = new ServiceCollection();

        // 註冊所有支付閘道
        services.AddTransient<StripePayment>();
        services.AddTransient<PayPalPayment>();
        services.AddTransient<LinePayPayment>();

        // 註冊工廠
        services.AddSingleton<IPaymentFactory, PaymentFactory>();
        services.AddTransient<OrderService>();

        var provider = services.BuildServiceProvider();
        var orderService = provider.GetRequiredService<OrderService>();

        Console.WriteLine("--- 顯示可用付款方式 ---");
        orderService.ShowAvailablePaymentMethods();

        Console.WriteLine("\n--- 使用不同付款方式 ---");
        orderService.PlaceOrder("Stripe", 1000);
        orderService.PlaceOrder("PayPal", 2000);
        orderService.PlaceOrder("LinePay", 3000);

        Console.WriteLine("\n--- 嘗試使用不存在的付款方式 ---");
        orderService.PlaceOrder("Unknown", 500);

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 使用工廠模式實現具名服務");
        Console.WriteLine("✅ 透過字典映射名稱到服務實例");
        Console.WriteLine("✅ 可以動態選擇要使用的實作");
        Console.WriteLine("✅ 適用於需要執行時選擇實作的場景");
    }
}
