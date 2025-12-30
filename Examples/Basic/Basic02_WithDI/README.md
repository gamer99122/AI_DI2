# Basic02: 使用 DI 改善 Basic01

## 學習目標

學習如何使用依賴注入來解決 Basic01 中的問題：
- 理解介面抽象化
- 學習建構函數注入
- 使用 ServiceCollection 管理依賴

## 關鍵改變

### 改變 1：定義介面

```csharp
// ✅ 定義抽象介面
public interface INotificationService
{
    void Send(string to, string subject, string message);
}

// ✅ 實作介面
public class EmailService : INotificationService
{
    // 實作...
}
```

### 改變 2：建構函數注入

```csharp
// ❌ Basic01: 直接 new
private readonly EmailService _emailService = new EmailService();

// ✅ Basic02: 建構函數注入
private readonly INotificationService _notificationService;

public UserService(INotificationService notificationService)
{
    _notificationService = notificationService;
}
```

### 改變 3：使用 DI 容器

```csharp
// 1. 建立容器
var services = new ServiceCollection();

// 2. 註冊服務
services.AddTransient<INotificationService, EmailService>();
services.AddTransient<UserService>();

// 3. 建立 Provider
var serviceProvider = services.BuildServiceProvider();

// 4. 解析服務
var userService = serviceProvider.GetRequiredService<UserService>();
```

## 優點對比

| 方面 | Basic01（無DI） | Basic02（有DI） |
|------|----------------|----------------|
| **耦合度** | 緊耦合 | 鬆耦合 |
| **可測試性** | 困難 | 容易 |
| **可擴展性** | 需修改程式碼 | 只需更換註冊 |
| **依賴管理** | 分散各處 | 集中管理 |
| **符合SOLID** | ❌ | ✅ |

## 執行結果

```
========================================
Basic02: 使用 DI 的程式碼範例
========================================

--- 測試 1: 註冊用戶 ---
[UserService] 註冊用戶: 張三
[UserService] 將用戶資料存入資料庫...
[EmailService] 發送郵件到: zhangsan@example.com
主旨: 歡迎註冊
內容: 親愛的 張三，歡迎加入！

[UserService] 用戶 張三 註冊成功！
```

## 如何替換實作？

假設現在要改用 SMS 發送通知：

```csharp
// 1. 實作 SMS 服務
public class SmsService : INotificationService
{
    public void Send(string to, string subject, string message)
    {
        Console.WriteLine($"[SMS] 發送簡訊到: {to}");
        Console.WriteLine($"內容: {message}");
    }
}

// 2. 只需修改註冊（無需修改 UserService）
services.AddTransient<INotificationService, SmsService>(); // ← 只改這裡！
```

**UserService 和 OrderService 的程式碼完全不需要修改！**

## DI 的四個步驟

1. **建立容器**：`new ServiceCollection()`
2. **註冊服務**：`services.AddXxx<TInterface, TImplementation>()`
3. **建立 Provider**：`services.BuildServiceProvider()`
4. **解析服務**：`serviceProvider.GetRequiredService<T>()`

## 下一步

- **Basic03**：學習 Transient 生命週期
- **Basic07**：更深入理解介面注入

## 關鍵概念

- ✅ 依賴介面（`INotificationService`）而非具體類別（`EmailService`）
- ✅ 透過建構函數注入依賴
- ✅ 使用 DI 容器管理物件創建
- ✅ 符合依賴反轉原則（DIP）
