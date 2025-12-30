// Advanced03: 模組化架構
using Microsoft.Extensions.DependencyInjection;

namespace Advanced03_ModularArchitecture;

// 模組介面
public interface IModule
{
    string Name { get; }
    void RegisterServices(IServiceCollection services);
}

// 用戶模組
public class UserModule : IModule
{
    public string Name => "User Module";

    public void RegisterServices(IServiceCollection services)
    {
        Console.WriteLine($"[{Name}] 註冊服務...");
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}

public interface IUserService { void Execute(); }
public class UserService : IUserService
{
    public void Execute() => Console.WriteLine("[UserService] 執行");
}
public interface IUserRepository { }
public class UserRepository : IUserRepository { }

// 訂單模組
public class OrderModule : IModule
{
    public string Name => "Order Module";

    public void RegisterServices(IServiceCollection services)
    {
        Console.WriteLine($"[{Name}] 註冊服務...");
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}

public interface IOrderService { void Execute(); }
public class OrderService : IOrderService
{
    public void Execute() => Console.WriteLine("[OrderService] 執行");
}
public interface IOrderRepository { }
public class OrderRepository : IOrderRepository { }

// 支付模組
public class PaymentModule : IModule
{
    public string Name => "Payment Module";

    public void RegisterServices(IServiceCollection services)
    {
        Console.WriteLine($"[{Name}] 註冊服務...");
        services.AddScoped<IPaymentService, PaymentService>();
    }
}

public interface IPaymentService { void Execute(); }
public class PaymentService : IPaymentService
{
    public void Execute() => Console.WriteLine("[PaymentService] 執行");
}

// 模組載入器
public class ModuleLoader
{
    private readonly List<IModule> _modules = new();

    public void AddModule(IModule module)
    {
        _modules.Add(module);
    }

    public void LoadModules(IServiceCollection services)
    {
        Console.WriteLine("=== 開始載入模組 ===\n");

        foreach (var module in _modules)
        {
            module.RegisterServices(services);
        }

        Console.WriteLine($"\n✅ 已載入 {_modules.Count} 個模組");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Advanced03: 模組化架構 ===\n");

        var services = new ServiceCollection();
        var loader = new ModuleLoader();

        // 動態載入模組
        loader.AddModule(new UserModule());
        loader.AddModule(new OrderModule());
        loader.AddModule(new PaymentModule());

        // 批次註冊所有模組的服務
        loader.LoadModules(services);

        var provider = services.BuildServiceProvider();

        Console.WriteLine("\n--- 測試服務 ---");
        var userService = provider.GetRequiredService<IUserService>();
        userService.Execute();

        var orderService = provider.GetRequiredService<IOrderService>();
        orderService.Execute();

        var paymentService = provider.GetRequiredService<IPaymentService>();
        paymentService.Execute();

        Console.WriteLine("\n✅ 模組化架構優點:");
        Console.WriteLine("  - 關注點分離");
        Console.WriteLine("  - 易於維護");
        Console.WriteLine("  - 動態載入/卸載");
        Console.WriteLine("  - 團隊協作");
    }
}
