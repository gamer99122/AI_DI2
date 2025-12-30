// Advanced02: 攔截器模式（AOP）
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Advanced02_InterceptorPattern;

// 原始服務
public interface IUserService
{
    void CreateUser(string name);
    string GetUser(int id);
}

public class UserService : IUserService
{
    public void CreateUser(string name)
    {
        Thread.Sleep(100); // 模擬耗時操作
        Console.WriteLine($"[UserService] 創建用戶: {name}");
    }

    public string GetUser(int id)
    {
        Thread.Sleep(50); // 模擬耗時操作
        return $"User_{id}";
    }
}

// 效能監控攔截器
public class PerformanceInterceptor<T> where T : class
{
    private readonly T _inner;

    public PerformanceInterceptor(T inner)
    {
        _inner = inner;
    }

    public TResult Intercept<TResult>(Func<T, TResult> action, string methodName)
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"[Interceptor] 開始執行: {methodName}");

        try
        {
            var result = action(_inner);
            sw.Stop();
            Console.WriteLine($"[Interceptor] 完成執行: {methodName}，耗時: {sw.ElapsedMilliseconds}ms\n");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Interceptor] 執行失敗: {methodName}，錯誤: {ex.Message}\n");
            throw;
        }
    }

    public void Intercept(Action<T> action, string methodName)
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"[Interceptor] 開始執行: {methodName}");

        try
        {
            action(_inner);
            sw.Stop();
            Console.WriteLine($"[Interceptor] 完成執行: {methodName}，耗時: {sw.ElapsedMilliseconds}ms\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Interceptor] 執行失敗: {methodName}，錯誤: {ex.Message}\n");
            throw;
        }
    }
}

// 使用攔截器的代理類
public class UserServiceProxy : IUserService
{
    private readonly PerformanceInterceptor<IUserService> _interceptor;

    public UserServiceProxy(IUserService inner)
    {
        _interceptor = new PerformanceInterceptor<IUserService>(inner);
    }

    public void CreateUser(string name)
    {
        _interceptor.Intercept(service => service.CreateUser(name), nameof(CreateUser));
    }

    public string GetUser(int id)
    {
        return _interceptor.Intercept(service => service.GetUser(id), nameof(GetUser));
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Advanced02: 攔截器模式（AOP）===\n");

        var services = new ServiceCollection();

        // 註冊原始服務
        services.AddTransient<UserService>();

        // 註冊代理（攔截器）
        services.AddTransient<IUserService>(sp =>
        {
            var userService = sp.GetRequiredService<UserService>();
            return new UserServiceProxy(userService);
        });

        var provider = services.BuildServiceProvider();
        var userService = provider.GetRequiredService<IUserService>();

        Console.WriteLine("--- 測試攔截 ---");
        userService.CreateUser("張三");

        var user = userService.GetUser(123);
        Console.WriteLine($"取得用戶: {user}\n");

        Console.WriteLine("✅ 攔截器可用於:");
        Console.WriteLine("  - 效能監控");
        Console.WriteLine("  - 日誌記錄");
        Console.WriteLine("  - 快取");
        Console.WriteLine("  - 權限檢查");
        Console.WriteLine("  - 交易管理");
    }
}
