# 依賴注入常見問題 FAQ

## 基礎問題

### Q1: 什麼時候應該使用 DI？

**A:** 當你的程式碼符合以下任一情況時：
- ✅ 需要單元測試
- ✅ 類別依賴外部資源（資料庫、API、檔案系統）
- ✅ 有多種實作方式需要切換
- ✅ 希望降低類別之間的耦合度
- ✅ 團隊協作開發

**不需要 DI 的情況：**
- ❌ 簡單的 DTO/Model 類別
- ❌ 純數學計算的工具類別
- ❌ 小型一次性腳本

---

### Q2: 為什麼不直接 `new` 物件？

```csharp
// ❌ 直接 new
public class OrderService
{
    private PaymentService _payment = new PaymentService();
}

// ✅ 使用 DI
public class OrderService
{
    private readonly IPaymentService _payment;

    public OrderService(IPaymentService payment)
    {
        _payment = payment;
    }
}
```

**直接 new 的問題：**
1. **緊耦合**：`OrderService` 綁定到 `PaymentService` 的具體實作
2. **難測試**：無法在測試時替換為 Mock 物件
3. **難維護**：修改 `PaymentService` 可能影響所有使用它的類別
4. **違反 SOLID**：違反依賴反轉原則

---

### Q3: Interface 一定要用嗎？

**A:** 不一定，但強烈建議。

**不使用 Interface 的情況：**
```csharp
services.AddScoped<MyService>();  // 直接註冊具體類別
```

**使用 Interface 的好處：**
```csharp
services.AddScoped<IMyService, MyService>();
```
- ✅ 易於替換實作
- ✅ 易於單元測試（Mock）
- ✅ 明確的契約定義
- ✅ 符合依賴反轉原則

**何時可以不用 Interface：**
- 確定不會有多種實作
- 不需要單元測試
- 內部使用的簡單類別

---

### Q4: 三種生命週期該怎麼選？

```csharp
// Transient：每次都新建
services.AddTransient<IEmailService, EmailService>();

// Scoped：同一範圍共享（Web 請求內共享）
services.AddScoped<IDbContext, MyDbContext>();

// Singleton：全域單例
services.AddSingleton<ICache, MemoryCache>();
```

**快速選擇指南：**
- 📧 無狀態服務 → **Transient**
- 🗄️ 資料庫操作 → **Scoped**
- 💾 快取、設定 → **Singleton**

**預設建議：**
不確定時，選擇 **Scoped**，這是最安全的選擇。

---

## 註冊問題

### Q5: 一個介面可以註冊多個實作嗎？

**A:** 可以！

```csharp
public interface INotificationService
{
    void Send(string message);
}

public class EmailNotification : INotificationService
{
    public void Send(string message) => Console.WriteLine($"Email: {message}");
}

public class SmsNotification : INotificationService
{
    public void Send(string message) => Console.WriteLine($"SMS: {message}");
}

// 註冊多個實作
services.AddTransient<INotificationService, EmailNotification>();
services.AddTransient<INotificationService, SmsNotification>();

// 取得所有實作
var notifications = serviceProvider.GetServices<INotificationService>(); // 回傳兩個
foreach (var notification in notifications)
{
    notification.Send("Hello");
}

// 取得最後註冊的
var notification = serviceProvider.GetService<INotificationService>(); // 只回傳 SmsNotification
```

---

### Q6: 註冊順序重要嗎？

**A:** 大部分情況不重要，但有例外。

**例外情況 1：多個實作**
```csharp
services.AddTransient<IService, ServiceA>();
services.AddTransient<IService, ServiceB>();

var service = serviceProvider.GetService<IService>(); // 取得 ServiceB（最後註冊的）
```

**例外情況 2：TryAdd 系列方法**
```csharp
services.TryAddTransient<IService, ServiceA>(); // 註冊成功
services.TryAddTransient<IService, ServiceB>(); // 被忽略（已有註冊）

var service = serviceProvider.GetService<IService>(); // 取得 ServiceA
```

---

### Q7: TryAdd 和 Add 有什麼差別？

```csharp
// Add：總是添加
services.AddTransient<IService, MyService>();
services.AddTransient<IService, MyService>(); // 會註冊兩次

// TryAdd：只在沒有註冊時才添加
services.TryAddTransient<IService, MyService>();
services.TryAddTransient<IService, MyService>(); // 被忽略

// TryAddEnumerable：避免重複註冊同一個實作
services.TryAddEnumerable(ServiceDescriptor.Transient<IService, MyService>());
services.TryAddEnumerable(ServiceDescriptor.Transient<IService, MyService>()); // 被忽略
```

**使用場景：**
- `Add`：確定要註冊的服務
- `TryAdd`：提供預設實作，允許使用者覆蓋
- `TryAddEnumerable`：註冊多個實作時避免重複

---

## 解析問題

### Q8: GetService 和 GetRequiredService 的差別？

```csharp
// GetService：服務不存在時回傳 null
var service = serviceProvider.GetService<IMyService>();
if (service == null)
{
    // 處理服務不存在的情況
}

// GetRequiredService：服務不存在時拋出例外
var service = serviceProvider.GetRequiredService<IMyService>(); // 不存在會拋出 InvalidOperationException
```

**建議：**
- 必要的服務用 `GetRequiredService`（大多數情況）
- 可選的服務用 `GetService`

---

### Q9: 可以在建構函數中使用 IServiceProvider 嗎？

```csharp
// ⚠️ 不推薦（Service Locator 反模式）
public class MyService
{
    private readonly IServiceProvider _serviceProvider;

    public MyService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void DoWork()
    {
        var dependency = _serviceProvider.GetService<IDependency>();
    }
}

// ✅ 推薦（明確的依賴）
public class MyService
{
    private readonly IDependency _dependency;

    public MyService(IDependency dependency)
    {
        _dependency = dependency;
    }

    public void DoWork()
    {
        // 使用 _dependency
    }
}
```

**例外情況：**
只有在需要動態解析或工廠模式時才使用 `IServiceProvider`。

---

### Q10: 如何注入多個相同介面的不同實作？

**方式 1：使用 IEnumerable**
```csharp
public class NotificationManager
{
    private readonly IEnumerable<INotificationService> _notifications;

    public NotificationManager(IEnumerable<INotificationService> notifications)
    {
        _notifications = notifications;
    }

    public void SendAll(string message)
    {
        foreach (var notification in _notifications)
        {
            notification.Send(message);
        }
    }
}
```

**方式 2：使用具名註冊（需要第三方套件或自行實作）**
```csharp
// 使用工廠模式
services.AddTransient<EmailNotification>();
services.AddTransient<SmsNotification>();
services.AddTransient<INotificationFactory, NotificationFactory>();

public class NotificationFactory : INotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationService Create(string type)
    {
        return type switch
        {
            "email" => _serviceProvider.GetService<EmailNotification>(),
            "sms" => _serviceProvider.GetService<SmsNotification>(),
            _ => throw new ArgumentException()
        };
    }
}
```

---

## 生命週期問題

### Q11: Singleton 可以注入 Scoped 服務嗎？

**A:** ❌ **絕對不行！** 這是最常見的錯誤。

```csharp
// ❌ 危險！
public class MySingletonService
{
    private readonly MyDbContext _dbContext; // DbContext 是 Scoped

    public MySingletonService(MyDbContext dbContext)
    {
        _dbContext = dbContext; // 錯誤！DbContext 會被持有整個應用程式生命週期
    }
}
```

**後果：**
- 記憶體洩漏
- DbContext 無法正確釋放連線
- 多執行緒問題

**正確做法：**
```csharp
public class MySingletonService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MySingletonService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void DoWork()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        // 使用 dbContext
    }
}
```

---

### Q12: Transient 服務會被 Dispose 嗎？

**A:** 會！由容器追蹤並在 Scope 結束時釋放。

```csharp
public class MyTransientService : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine("Transient 被釋放了");
    }
}

services.AddTransient<MyTransientService>();

using (var scope = serviceProvider.CreateScope())
{
    var service1 = scope.ServiceProvider.GetService<MyTransientService>();
    var service2 = scope.ServiceProvider.GetService<MyTransientService>();
} // 離開 scope 時，service1 和 service2 都會被 Dispose

```

**注意：**
- Transient 服務會被容器追蹤，直到 Scope 結束
- 如果在 Root Provider 解析 Transient，會持續到應用程式結束（記憶體洩漏）

---

### Q13: 如何讓 Transient 不被追蹤？

**A:** 不從容器解析，而是手動創建：

```csharp
// ❌ 會被追蹤
var service = serviceProvider.GetService<MyTransientService>();

// ✅ 不被追蹤
var service = new MyTransientService();
using (service)
{
    // 使用服務
} // 手動釋放
```

或者使用 `ActivatorUtilities`：
```csharp
var service = ActivatorUtilities.CreateInstance<MyTransientService>(serviceProvider);
```

---

## ASP.NET Core 問題

### Q14: Controller 的生命週期是什麼？

**A:** Controller 本身是 **Transient**，每個請求都會創建新的實例。

```csharp
public class MyController : ControllerBase
{
    private static int _instanceCount = 0;

    public MyController()
    {
        var id = Interlocked.Increment(ref _instanceCount);
        Console.WriteLine($"Controller 實例 #{id} 建立");
    }
}
```

每個請求都會輸出不同的編號。

---

### Q15: Middleware 的生命週期是什麼？

**A:** Middleware 是 **Singleton**。

```csharp
public class MyMiddleware
{
    private readonly RequestDelegate _next;

    // ⚠️ 建構函數只會執行一次
    public MyMiddleware(RequestDelegate next)
    {
        _next = next;
        Console.WriteLine("Middleware 建立"); // 只輸出一次
    }

    // ✅ InvokeAsync 每個請求都會執行
    public async Task InvokeAsync(HttpContext context, IMyService service)
    {
        // service 可以是 Scoped
        await _next(context);
    }
}
```

**重點：**
- 建構函數只能注入 Singleton 服務
- `InvokeAsync` 可以注入 Scoped 服務

---

### Q16: 如何在 Minimal API 中使用 DI？

```csharp
var builder = WebApplication.CreateBuilder(args);

// 註冊服務
builder.Services.AddScoped<IMyService, MyService>();

var app = builder.Build();

// 方式 1：參數注入
app.MapGet("/api/data", (IMyService service) =>
{
    return service.GetData();
});

// 方式 2：多個參數
app.MapGet("/api/user/{id}", (int id, IMyService service, ILogger<Program> logger) =>
{
    logger.LogInformation($"Getting user {id}");
    return service.GetUser(id);
});

// 方式 3：HttpContext
app.MapGet("/api/info", (HttpContext context) =>
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    return service.GetInfo();
});
```

---

## 測試問題

### Q17: 如何在單元測試中使用 DI？

```csharp
// 使用 Moq 創建 Mock 物件
[Fact]
public void Test_UserService()
{
    // Arrange
    var mockEmailService = new Mock<IEmailService>();
    mockEmailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(true);

    var userService = new UserService(mockEmailService.Object);

    // Act
    var result = userService.RegisterUser("test@example.com");

    // Assert
    Assert.True(result);
    mockEmailService.Verify(x => x.Send("test@example.com", It.IsAny<string>()), Times.Once);
}
```

**整合測試：**
```csharp
public class IntegrationTest
{
    [Fact]
    public void Test_WithRealServices()
    {
        // 建立真實的 DI 容器
        var services = new ServiceCollection();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        // 測試
        var result = userService.RegisterUser("test@example.com");
        Assert.True(result);
    }
}
```

---

## 效能問題

### Q18: DI 會影響效能嗎？

**A:** 影響很小，幾乎可以忽略。

**效能比較：**
```csharp
// 直接 new
var service = new MyService(); // ~10 ns

// 從容器解析 Transient
var service = serviceProvider.GetService<MyService>(); // ~100 ns

// 從容器解析 Singleton
var service = serviceProvider.GetService<ISingleton>(); // ~50 ns
```

**結論：**
- 效能損失極小（奈秒級）
- 帶來的好處遠大於效能損失
- 真正的效能瓶頸通常在資料庫、網路 I/O

---

### Q19: 如何優化 DI 效能？

1. **使用 Singleton 快取昂貴物件**
```csharp
services.AddSingleton<IExpensiveService, ExpensiveService>();
```

2. **避免在迴圈中解析服務**
```csharp
// ❌ 不好
for (int i = 0; i < 1000; i++)
{
    var service = serviceProvider.GetService<IMyService>();
}

// ✅ 好
var service = serviceProvider.GetService<IMyService>();
for (int i = 0; i < 1000; i++)
{
    service.DoWork();
}
```

3. **使用 ObjectPool 重用物件**
```csharp
services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
services.AddSingleton(serviceProvider =>
{
    var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
    return provider.Create(new DefaultPooledObjectPolicy<MyObject>());
});
```

---

## 常見錯誤

### Q20: 循環依賴怎麼辦？

```csharp
// ❌ 循環依賴
public class ServiceA
{
    public ServiceA(ServiceB serviceB) { }
}

public class ServiceB
{
    public ServiceB(ServiceA serviceA) { } // 循環！
}
```

**錯誤訊息：**
```
A circular dependency was detected for the service of type 'ServiceA'.
```

**解決方案 1：重新設計（最佳）**
```csharp
// 提取共用邏輯到第三個服務
public class SharedService { }

public class ServiceA
{
    public ServiceA(SharedService shared) { }
}

public class ServiceB
{
    public ServiceB(SharedService shared) { }
}
```

**解決方案 2：使用介面打破循環**
```csharp
public class ServiceA : IServiceA
{
    public ServiceA(Lazy<IServiceB> serviceB) { }
}

public class ServiceB : IServiceB
{
    public ServiceB(Lazy<IServiceA> serviceA) { }
}
```

---

## 總結

依賴注入的黃金法則：
1. ✅ 透過建構函數注入
2. ✅ 依賴介面而非具體類別
3. ✅ 預設使用 Scoped
4. ❌ 避免 Service Locator 模式
5. ❌ 不要在 Singleton 中注入 Scoped
6. ✅ 啟用開發時驗證
7. ✅ 保持建構函數簡單
8. ✅ 適時使用工廠模式

遇到問題時：
1. 檢查生命週期是否正確
2. 啟用 `ValidateScopes` 和 `ValidateOnBuild`
3. 查看例外訊息的詳細堆疊追蹤
4. 使用 Debug 追蹤物件創建和釋放
