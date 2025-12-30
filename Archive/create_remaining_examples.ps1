# PowerShell 腳本：批次創建剩餘範例

$examples = @(
    @{N="Basic15"; T="IOptionsSnapshot"; D="範圍內的設定快照"},
    @{N="Basic16"; T="IHttpClientFactory"; D="HttpClient 工廠模式"},
    @{N="Basic17"; T="IHostedService"; D="背景服務"},
    @{N="Basic18"; T="IMemoryCache"; D="記憶體快取"},
    @{N="Basic19"; T="IDistributedCache"; D="分散式快取"},
    @{N="Basic20"; T="IServiceProvider"; D="ServiceProvider 使用"},
    @{N="Basic22"; T="ServiceLayer"; D="Service 層實作"},
    @{N="Basic23"; T="MultipleImplementations"; D="多個實作"},
    @{N="Basic24"; T="NamedServices"; D="具名服務"},
    @{N="Basic26"; T="DecoratorPattern"; D="裝飾者模式"},
    @{N="Basic27"; T="ChainOfResponsibility"; D="責任鏈模式"},
    @{N="Basic28"; T="StrategyPattern"; D="策略模式"},
    @{N="Basic29"; T="GenericServices"; D="泛型服務"},
    @{N="Basic30"; T="OpenGenericTypes"; D="開放泛型類型"},
    @{N="Basic31"; T="ControllerDI"; D="Controller 中使用 DI"},
    @{N="Basic32"; T="MinimalApiDI"; D="Minimal API 中使用 DI"},
    @{N="Basic33"; T="MiddlewareDI"; D="Middleware 中使用 DI"},
    @{N="Basic34"; T="FilterDI"; D="Filter 中使用 DI"},
    @{N="Basic35"; T="RazorPagesDI"; D="Razor Pages 中使用 DI"},
    @{N="Basic36"; T="BlazorDI"; D="Blazor 中使用 DI"},
    @{N="Basic37"; T="ApiVersioning"; D="API 版本控制"},
    @{N="Basic38"; T="ValidationService"; D="驗證服務"},
    @{N="Basic39"; T="ExceptionHandling"; D="例外處理服務"},
    @{N="Basic40"; T="DatabaseContext"; D="EF Core DbContext"}
)

foreach ($ex in $examples) {
    $dir = "Examples\Basic\$($ex.N)_$($ex.T)"
    New-Item -ItemType Directory -Force -Path $dir | Out-Null

    $code = @"
// $($ex.N): $($ex.D)
using Microsoft.Extensions.DependencyInjection;

namespace $($ex.N)_$($ex.T);

// TODO: 實作 $($ex.D) 範例

public interface IMyService
{
    void Execute();
}

public class MyService : IMyService
{
    public void Execute()
    {
        Console.WriteLine("[MyService] 執行 $($ex.D)");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== $($ex.N): $($ex.D) ===\n");

        var services = new ServiceCollection();
        services.AddTransient<IMyService, MyService>();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IMyService>();

        service.Execute();

        Console.WriteLine("\n✅ 範例展示: $($ex.D)");
    }
}
"@

    Set-Content -Path "$dir\Program.cs" -Value $code -Encoding UTF8

    $readme = @"
# $($ex.N): $($ex.D)

## 學習目標

學習 $($ex.D) 的使用方式。

## 執行

``````bash
cd $dir
dotnet run
``````

## 關鍵概念

- $($ex.D)
- 依賴注入應用

"@

    Set-Content -Path "$dir\README.md" -Value $readme -Encoding UTF8
}

Write-Host "已創建 $($examples.Count) 個範例" -ForegroundColor Green
