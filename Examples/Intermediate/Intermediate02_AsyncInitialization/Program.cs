// Intermediate02: 非同步初始化
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intermediate02_AsyncInitialization;

// 需要非同步初始化的服務
public interface IDatabaseInitializer
{
    Task InitializeAsync();
    bool IsInitialized { get; }
}

public class DatabaseInitializer : IDatabaseInitializer
{
    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync()
    {
        Console.WriteLine("[Database] 開始初始化...");
        await Task.Delay(1000); // 模擬非同步操作
        Console.WriteLine("[Database] 初始化完成");
        IsInitialized = true;
    }
}

// 使用 IHostedService 進行初始化
public class InitializationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public InitializationHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[App] 應用程式啟動中...\n");

        using var scope = _serviceProvider.CreateScope();
        var dbInit = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

        await dbInit.InitializeAsync();

        Console.WriteLine("\n[App] 所有初始化完成！");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[App] 應用程式關閉");
        return Task.CompletedTask;
    }
}

// 業務服務
public class UserService
{
    private readonly IDatabaseInitializer _dbInit;

    public UserService(IDatabaseInitializer dbInit)
    {
        _dbInit = dbInit;
    }

    public void CreateUser(string name)
    {
        if (!_dbInit.IsInitialized)
        {
            Console.WriteLine("[Error] 資料庫尚未初始化！");
            return;
        }

        Console.WriteLine($"[UserService] 創建用戶: {name}");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Intermediate02: 非同步初始化 ===\n");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
                services.AddHostedService<InitializationHostedService>();
                services.AddScoped<UserService>();
            })
            .Build();

        // 啟動並等待初始化完成
        await host.StartAsync();

        // 使用服務
        using (var scope = host.Services.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            userService.CreateUser("張三");
        }

        Console.WriteLine("\n✅ 使用 IHostedService 進行非同步初始化");
        Console.WriteLine("✅ 確保服務啟動前完成必要的初始化");

        await host.StopAsync();
    }
}
