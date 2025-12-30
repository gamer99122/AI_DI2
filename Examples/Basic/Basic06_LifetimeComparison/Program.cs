// Basic06: 三種生命週期比較
using Microsoft.Extensions.DependencyInjection;

namespace Basic06_LifetimeComparison;

public interface IService { Guid Id { get; } }
public class MyService : IService { public Guid Id { get; } = Guid.NewGuid(); }

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic06: 生命週期比較 ===\n");

        // 三個容器，分別測試三種生命週期
        var services1 = new ServiceCollection();
        services1.AddTransient<IService, MyService>();
        var provider1 = services1.BuildServiceProvider();

        var services2 = new ServiceCollection();
        services2.AddScoped<IService, MyService>();
        var provider2 = services2.BuildServiceProvider();

        var services3 = new ServiceCollection();
        services3.AddSingleton<IService, MyService>();
        var provider3 = services3.BuildServiceProvider();

        Console.WriteLine("--- Transient ---");
        Console.WriteLine($"第1次: {provider1.GetService<IService>()!.Id}");
        Console.WriteLine($"第2次: {provider1.GetService<IService>()!.Id}");
        Console.WriteLine("❌ 每次都不同\n");

        Console.WriteLine("--- Scoped ---");
        using (var scope = provider2.CreateScope())
        {
            Console.WriteLine($"Scope內第1次: {scope.ServiceProvider.GetService<IService>()!.Id}");
            Console.WriteLine($"Scope內第2次: {scope.ServiceProvider.GetService<IService>()!.Id}");
            Console.WriteLine("✅ Scope內相同");
        }
        using (var scope = provider2.CreateScope())
        {
            Console.WriteLine($"新Scope:     {scope.ServiceProvider.GetService<IService>()!.Id}");
            Console.WriteLine("❌ 不同Scope不同\n");
        }

        Console.WriteLine("--- Singleton ---");
        Console.WriteLine($"第1次: {provider3.GetService<IService>()!.Id}");
        Console.WriteLine($"第2次: {provider3.GetService<IService>()!.Id}");
        Console.WriteLine($"第3次: {provider3.GetService<IService>()!.Id}");
        Console.WriteLine("✅ 永遠相同");

        Console.WriteLine("\n快速記憶：");
        Console.WriteLine("Transient = 一次性紙杯（每次新的）");
        Console.WriteLine("Scoped    = 個人水壺（一個請求內共用）");
        Console.WriteLine("Singleton = 飲水機（大家共用一個）");
    }
}
