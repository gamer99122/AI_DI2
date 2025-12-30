// Basic39: 例外處理服務
using Microsoft.Extensions.DependencyInjection;

namespace Basic39_ExceptionHandling;

// 例外處理服務介面
public interface IExceptionHandler
{
    void Handle(Exception exception);
}

// 日誌服務
public interface ILogger
{
    void LogError(string message, Exception exception);
}

public class ConsoleLogger : ILogger
{
    public void LogError(string message, Exception exception)
    {
        Console.WriteLine($"[ERROR] {message}");
        Console.WriteLine($"  類型: {exception.GetType().Name}");
        Console.WriteLine($"  訊息: {exception.Message}");
    }
}

// 例外處理器實作
public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public ExceptionHandler(ILogger logger)
    {
        _logger = logger;
    }

    public void Handle(Exception exception)
    {
        Console.WriteLine("\n[ExceptionHandler] 處理例外...");

        switch (exception)
        {
            case ArgumentNullException argEx:
                _logger.LogError("參數為 null", argEx);
                Console.WriteLine("  建議: 檢查參數是否正確傳入");
                break;

            case InvalidOperationException opEx:
                _logger.LogError("無效的操作", opEx);
                Console.WriteLine("  建議: 檢查操作順序");
                break;

            case DivideByZeroException divEx:
                _logger.LogError("除以零錯誤", divEx);
                Console.WriteLine("  建議: 檢查除數是否為零");
                break;

            default:
                _logger.LogError("未預期的錯誤", exception);
                Console.WriteLine("  建議: 聯繫技術支援");
                break;
        }
    }
}

// 業務服務
public class CalculatorService
{
    private readonly IExceptionHandler _exceptionHandler;

    public CalculatorService(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    public decimal Divide(decimal a, decimal b)
    {
        try
        {
            if (b == 0)
            {
                throw new DivideByZeroException("除數不可為零");
            }

            return a / b;
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex);
            return 0;
        }
    }

    public void ProcessData(string? data)
    {
        try
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "資料不可為 null");
            }

            Console.WriteLine($"處理資料: {data}");
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic39: 例外處理服務 ===\n");

        var services = new ServiceCollection();

        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddScoped<IExceptionHandler, ExceptionHandler>();
        services.AddScoped<CalculatorService>();

        var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var calculator = scope.ServiceProvider.GetRequiredService<CalculatorService>();

            Console.WriteLine("--- 測試 1: 正常除法 ---");
            var result1 = calculator.Divide(10, 2);
            Console.WriteLine($"結果: {result1}");

            Console.WriteLine("\n--- 測試 2: 除以零 ---");
            var result2 = calculator.Divide(10, 0);
            Console.WriteLine($"結果: {result2}");

            Console.WriteLine("\n--- 測試 3: Null 參數 ---");
            calculator.ProcessData(null);

            Console.WriteLine("\n--- 測試 4: 正常資料 ---");
            calculator.ProcessData("有效資料");
        }

        Console.WriteLine("\n\n重點：");
        Console.WriteLine("✅ 集中處理例外");
        Console.WriteLine("✅ 統一的錯誤記錄");
        Console.WriteLine("✅ 易於維護和擴展");
        Console.WriteLine("✅ 可以根據例外類型採取不同處理");
    }
}
