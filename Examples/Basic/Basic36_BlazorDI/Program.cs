// Basic36: Blazor 中使用 DI
using Microsoft.Extensions.DependencyInjection;

namespace Basic36_BlazorDI;

// 資料服務
public interface IDataService
{
    Task<List<string>> GetDataAsync();
}

public class DataService : IDataService
{
    public async Task<List<string>> GetDataAsync()
    {
        Console.WriteLine("[DataService] 載入資料...");
        await Task.Delay(100); // 模擬非同步操作

        return new List<string>
        {
            "項目 1",
            "項目 2",
            "項目 3"
        };
    }
}

// 狀態管理服務
public class AppState
{
    private string _currentUser = "訪客";

    public string CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Blazor 元件範例（模擬）
public class CounterComponent
{
    private readonly IDataService _dataService;
    private readonly AppState _appState;

    // ✅ 在 Blazor 元件中使用 @inject 或建構函數注入
    public CounterComponent(IDataService dataService, AppState appState)
    {
        _dataService = dataService;
        _appState = appState;
    }

    public async Task OnInitializedAsync()
    {
        Console.WriteLine($"[Component] 初始化，當前用戶: {_appState.CurrentUser}");
        var data = await _dataService.GetDataAsync();
        Console.WriteLine($"[Component] 載入了 {data.Count} 筆資料");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Basic36: Blazor DI ===\n");

        var services = new ServiceCollection();

        // ✅ 在 Blazor 中註冊服務
        services.AddScoped<IDataService, DataService>();
        services.AddSingleton<AppState>(); // 狀態通常是 Singleton

        var provider = services.BuildServiceProvider();

        Console.WriteLine("--- 模擬 Blazor 元件使用 DI ---");
        using (var scope = provider.CreateScope())
        {
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
            var appState = scope.ServiceProvider.GetRequiredService<AppState>();

            var component = new CounterComponent(dataService, appState);
            await component.OnInitializedAsync();

            appState.CurrentUser = "張三";
            Console.WriteLine($"[AppState] 用戶變更為: {appState.CurrentUser}");
        }

        Console.WriteLine("\nBlazor DI 重點：");
        Console.WriteLine("✅ 使用 @inject 指示詞注入服務");
        Console.WriteLine("✅ 元件是 Scoped（每個電路一個實例）");
        Console.WriteLine("✅ 狀態管理服務通常是 Singleton");
        Console.WriteLine("✅ HttpClient 使用 AddHttpClient");
    }
}
