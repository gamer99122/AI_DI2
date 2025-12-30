# Basic01: 沒有使用 DI 的程式碼

## 學習目標

理解傳統方式（直接 `new` 物件）的問題，為學習 DI 打下基礎。

## 程式碼說明

這個範例展示了一個**沒有使用依賴注入**的典型應用程式，包含三個類別：

1. **EmailService**：負責發送郵件
2. **UserService**：處理用戶註冊，直接 `new EmailService()`
3. **OrderService**：處理訂單建立，也直接 `new EmailService()`

## 問題分析

### 問題 1：緊耦合（Tight Coupling）

```csharp
public class UserService
{
    private readonly EmailService _emailService = new EmailService(); // 緊密耦合
}
```

`UserService` 直接依賴 `EmailService` 的具體實作，兩者緊密綁定。

### 問題 2：難以擴展

如果需要支援 SMS 通知，必須修改 `UserService` 和 `OrderService` 的程式碼：

```csharp
// 需要修改所有使用 EmailService 的地方
private readonly SmsService _smsService = new SmsService();
```

### 問題 3：難以測試

單元測試時無法替換 `EmailService` 為 Mock 物件，導致測試會真的發送郵件。

### 問題 4：違反 SOLID 原則

- **S**ingle Responsibility：`UserService` 不僅要處理用戶邏輯，還要負責創建 `EmailService`
- **O**pen/Closed：無法在不修改程式碼的情況下擴展
- **D**ependency Inversion：依賴具體類別而非抽象

## 執行結果

```
========================================
Basic01: 沒有使用 DI 的程式碼範例
========================================

--- 測試 1: 註冊用戶 ---
[UserService] 註冊用戶: 張三
[UserService] 將用戶資料存入資料庫...
[EmailService] 發送郵件到: zhangsan@example.com
主旨: 歡迎註冊
內容: 親愛的 張三，歡迎加入！

[UserService] 用戶 張三 註冊成功！

--- 測試 2: 建立訂單 ---
[OrderService] 建立訂單: #1001
[OrderService] 金額: $299.99
[EmailService] 發送郵件到: lisi@example.com
主旨: 訂單確認
內容: 您的訂單 #1001 已成功建立，金額: $299.99
[OrderService] 訂單 #1001 建立成功！
```

## 下一步

請繼續查看 **Basic02_WithDI**，看看使用依賴注入如何解決這些問題。

## 關鍵概念

- ❌ 直接 `new` 依賴 = 緊耦合
- ❌ 每個類別都創建自己的依賴 = 難以管理
- ❌ 依賴具體類別 = 違反 DIP
- ➡️ 解決方案：依賴注入（Dependency Injection）
