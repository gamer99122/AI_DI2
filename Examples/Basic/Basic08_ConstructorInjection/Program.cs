// Basic08: 建構函數注入（推薦方式）
using Microsoft.Extensions.DependencyInjection;

namespace Basic08_ConstructorInjection;

public interface ILogger { void Log(string message); }
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

public interface IRepository { void Save(); }
public class Repository : IRepository
{
    public void Save() => Console.WriteLine("[DB] 儲存資料");
}

// 建構函數注入多個依賴
public class UserService
{
    private readonly ILogger _logger;
    private readonly IRepository _repository;

    // ✅ 推薦：建構函數注入
    public UserService(ILogger logger, IRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public void CreateUser(string name)
    {
        _logger.Log($"建立用戶: {name}");
        _repository.Save();
        _logger.Log("完成");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic08: 建構函數注入 ===\n");

        var services = new ServiceCollection();
        services.AddTransient<ILogger, ConsoleLogger>();
        services.AddTransient<IRepository, Repository>();
        services.AddTransient<UserService>(); // DI 容器會自動注入依賴

        var provider = services.BuildServiceProvider();
        var userService = provider.GetRequiredService<UserService>();

        userService.CreateUser("張三");

        Console.WriteLine("\n優點：");
        Console.WriteLine("✅ 明確顯示所有依賴");
        Console.WriteLine("✅ 確保依賴不為 null");
        Console.WriteLine("✅ 支援 readonly（不可變）");
        Console.WriteLine("✅ 易於測試");
    }
}
