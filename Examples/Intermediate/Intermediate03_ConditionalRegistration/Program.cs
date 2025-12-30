// Intermediate03: 條件式服務註冊
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Intermediate03_ConditionalRegistration;

public interface IPaymentGateway { void Process(decimal amount); }

public class StripePayment : IPaymentGateway
{
    public void Process(decimal amount) => Console.WriteLine($"[Stripe] 處理 ${amount}");
}

public class PayPalPayment : IPaymentGateway
{
    public void Process(decimal amount) => Console.WriteLine($"[PayPal] 處理 ${amount}");
}

public class FakePayment : IPaymentGateway
{
    public void Process(decimal amount) => Console.WriteLine($"[Fake] 模擬處理 ${amount}");
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Intermediate03: 條件式註冊 ===\n");

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Environment"] = "Development", // 或 "Production"
                ["PaymentGateway"] = "PayPal"   // 或 "Stripe"
            })
            .Build();

        var services = new ServiceCollection();

        // ✅ 根據環境註冊
        var environment = config["Environment"];
        if (environment == "Development")
        {
            Console.WriteLine("開發環境：使用 Fake Payment");
            services.AddSingleton<IPaymentGateway, FakePayment>();
        }
        else
        {
            // 根據設定選擇支付閘道
            var gateway = config["PaymentGateway"];
            if (gateway == "Stripe")
            {
                Console.WriteLine("生產環境：使用 Stripe");
                services.AddSingleton<IPaymentGateway, StripePayment>();
            }
            else
            {
                Console.WriteLine("生產環境：使用 PayPal");
                services.AddSingleton<IPaymentGateway, PayPalPayment>();
            }
        }

        var provider = services.BuildServiceProvider();
        var payment = provider.GetRequiredService<IPaymentGateway>();

        payment.Process(99.99m);

        Console.WriteLine("\n✅ 根據環境/設定動態註冊服務");
        Console.WriteLine("✅ 開發環境使用測試實作");
        Console.WriteLine("✅ 生產環境使用真實實作");
    }
}
