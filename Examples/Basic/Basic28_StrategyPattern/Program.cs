// Basic28: 策略模式
using Microsoft.Extensions.DependencyInjection;

namespace Basic28_StrategyPattern;

// 折扣策略介面
public interface IDiscountStrategy
{
    string Name { get; }
    decimal CalculateDiscount(decimal originalPrice);
}

// 策略 1: 無折扣
public class NoDiscountStrategy : IDiscountStrategy
{
    public string Name => "原價";
    public decimal CalculateDiscount(decimal originalPrice) => 0;
}

// 策略 2: 百分比折扣
public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percentage;

    public string Name => $"{_percentage * 100}% 折扣";

    public PercentageDiscountStrategy(decimal percentage = 0.1m)
    {
        _percentage = percentage;
    }

    public decimal CalculateDiscount(decimal originalPrice)
    {
        return originalPrice * _percentage;
    }
}

// 策略 3: 固定金額折扣
public class FixedAmountDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _amount;

    public string Name => $"折扣 ${_amount}";

    public FixedAmountDiscountStrategy(decimal amount = 100)
    {
        _amount = amount;
    }

    public decimal CalculateDiscount(decimal originalPrice)
    {
        return Math.Min(_amount, originalPrice);
    }
}

// 購物車
public class ShoppingCart
{
    private IDiscountStrategy _discountStrategy;

    public ShoppingCart(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public void SetDiscountStrategy(IDiscountStrategy strategy)
    {
        _discountStrategy = strategy;
    }

    public void Checkout(decimal totalPrice)
    {
        var discount = _discountStrategy.CalculateDiscount(totalPrice);
        var finalPrice = totalPrice - discount;

        Console.WriteLine($"\n購物車結帳:");
        Console.WriteLine($"  原價: ${totalPrice}");
        Console.WriteLine($"  折扣策略: {_discountStrategy.Name}");
        Console.WriteLine($"  折扣金額: ${discount}");
        Console.WriteLine($"  最終價格: ${finalPrice}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic28: 策略模式 ===\n");

        var services = new ServiceCollection();

        // 註冊不同策略
        services.AddTransient<NoDiscountStrategy>();
        services.AddTransient<PercentageDiscountStrategy>(sp =>
            new PercentageDiscountStrategy(0.2m)); // 20% 折扣
        services.AddTransient<FixedAmountDiscountStrategy>(sp =>
            new FixedAmountDiscountStrategy(500)); // 折 $500

        var provider = services.BuildServiceProvider();

        var totalPrice = 2000m;

        Console.WriteLine("--- 策略 1: 無折扣 ---");
        var noDiscount = provider.GetRequiredService<NoDiscountStrategy>();
        var cart1 = new ShoppingCart(noDiscount);
        cart1.Checkout(totalPrice);

        Console.WriteLine("\n--- 策略 2: 百分比折扣 ---");
        var percentageDiscount = provider.GetRequiredService<PercentageDiscountStrategy>();
        var cart2 = new ShoppingCart(percentageDiscount);
        cart2.Checkout(totalPrice);

        Console.WriteLine("\n--- 策略 3: 固定金額折扣 ---");
        var fixedDiscount = provider.GetRequiredService<FixedAmountDiscountStrategy>();
        var cart3 = new ShoppingCart(fixedDiscount);
        cart3.Checkout(totalPrice);

        Console.WriteLine("\n--- 動態切換策略 ---");
        var cart = new ShoppingCart(noDiscount);
        cart.Checkout(totalPrice);

        cart.SetDiscountStrategy(percentageDiscount);
        cart.Checkout(totalPrice);

        Console.WriteLine("\n重點：");
        Console.WriteLine("✅ 策略模式封裝演算法");
        Console.WriteLine("✅ 可以動態切換策略");
        Console.WriteLine("✅ 符合開放封閉原則");
    }
}
