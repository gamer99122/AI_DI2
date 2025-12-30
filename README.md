# .NET 8 依賴注入（DI）完整教學

## 專案說明

本專案提供 50 個從淺入深的依賴注入範例，適合第一次學習 DI 的初學者。

## 什麼是依賴注入（Dependency Injection）？

依賴注入是一種設計模式，用於實現控制反轉（IoC, Inversion of Control）。它能讓程式碼更：
- **鬆耦合**：類別之間的依賴關係減少
- **易測試**：可以輕鬆替換依賴項進行單元測試
- **易維護**：修改實作時不需要改動使用者的程式碼

## 學習路徑

### 📗 基礎範例（Basic 01-40）
適合完全沒有 DI 經驗的初學者，從最基本的概念開始。

### 📘 中階範例（Intermediate 01-05）
掌握基礎後，學習更實用的進階技巧。

### 📕 進階範例（Advanced 01-05）
深入理解 DI 的高級應用場景。

---

## 📗 基礎範例清單（40個）

### 核心概念（1-10）
1. **Basic01_WithoutDI** - 沒有使用 DI 的程式碼（看看問題在哪）
2. **Basic02_WithDI** - 使用 DI 改善 Basic01
3. **Basic03_TransientLifetime** - Transient 生命週期介紹
4. **Basic04_ScopedLifetime** - Scoped 生命週期介紹
5. **Basic05_SingletonLifetime** - Singleton 生命週期介紹
6. **Basic06_LifetimeComparison** - 三種生命週期比較
7. **Basic07_InterfaceInjection** - 透過介面注入
8. **Basic08_ConstructorInjection** - 建構函數注入
9. **Basic09_MultipleServices** - 註冊多個服務
10. **Basic10_ServiceResolution** - 服務解析

### 常用服務注入（11-20）
11. **Basic11_ILoggerInjection** - ILogger 日誌注入
12. **Basic12_IConfigurationInjection** - IConfiguration 設定注入
13. **Basic13_IOptionsInjection** - IOptions 強型別設定
14. **Basic14_IOptionsMonitor** - IOptionsMonitor 動態設定
15. **Basic15_IOptionsSnapshot** - IOptionsSnapshot 範圍設定
16. **Basic16_IHttpClientFactory** - HttpClient 工廠模式
17. **Basic17_IHostedService** - 背景服務
18. **Basic18_IMemoryCache** - 記憶體快取
19. **Basic19_IDistributedCache** - 分散式快取
20. **Basic20_IServiceProvider** - ServiceProvider 使用

### 實作模式（21-30）
21. **Basic21_RepositoryPattern** - Repository 模式
22. **Basic22_ServiceLayer** - Service 層實作
23. **Basic23_MultipleImplementations** - 一個介面多個實作
24. **Basic24_NamedServices** - 具名服務注入
25. **Basic25_FactoryPattern** - 工廠模式與 DI
26. **Basic26_DecoratorPattern** - 裝飾者模式
27. **Basic27_ChainOfResponsibility** - 責任鏈模式
28. **Basic28_StrategyPattern** - 策略模式
29. **Basic29_GenericServices** - 泛型服務
30. **Basic30_OpenGenericTypes** - 開放泛型類型

### Web 應用（31-40）
31. **Basic31_ControllerDI** - Controller 中使用 DI
32. **Basic32_MinimalApiDI** - Minimal API 中使用 DI
33. **Basic33_MiddlewareDI** - Middleware 中使用 DI
34. **Basic34_FilterDI** - Filter 中使用 DI
35. **Basic35_RazorPagesDI** - Razor Pages 中使用 DI
36. **Basic36_BlazorDI** - Blazor 中使用 DI
37. **Basic37_ApiVersioning** - API 版本控制
38. **Basic38_ValidationService** - 驗證服務
39. **Basic39_ExceptionHandling** - 例外處理服務
40. **Basic40_DatabaseContext** - EF Core DbContext

---

## 📘 中階範例清單（5個）

41. **Intermediate01_ScopedFactory** - Scoped 服務工廠模式
42. **Intermediate02_AsyncInitialization** - 非同步初始化
43. **Intermediate03_ConditionalRegistration** - 條件式服務註冊
44. **Intermediate04_ServiceLifetimeValidation** - 生命週期驗證
45. **Intermediate05_MultitenantDI** - 多租戶架構

---

## 📕 進階範例清單（5個）

46. **Advanced01_CustomServiceProvider** - 自訂 ServiceProvider
47. **Advanced02_InterceptorPattern** - 攔截器模式（AOP）
48. **Advanced03_ModularArchitecture** - 模組化架構
49. **Advanced04_PluginSystem** - 插件系統
50. **Advanced05_PerformanceOptimization** - 效能優化技巧

---

## 如何使用本專案

### 環境需求
- .NET 8.0 SDK 或更高版本
- Visual Studio 2022 / VS Code / Rider

### 執行範例

每個範例都是獨立的程式，可以直接執行：

```bash
# 進入範例目錄
cd Examples/Basic01_WithoutDI

# 執行
dotnet run
```

### 學習建議

1. **按順序學習**：從 Basic01 開始，循序漸進
2. **動手實作**：看懂後自己重新寫一遍
3. **修改實驗**：試著修改程式碼，觀察結果
4. **閱讀註解**：每個範例都有詳細的中文註解
5. **比較差異**：對比有 DI 和無 DI 的差異

---

## 專案結構

```
AI_DI2/
├── README.md                          # 本文件
├── DI_Tutorial.sln                    # Solution 文件
├── Docs/                              # 說明文檔
│   ├── 01_DI基礎概念.md
│   ├── 02_生命週期詳解.md
│   ├── 03_常見問題FAQ.md
│   └── 04_最佳實踐.md
├── Examples/                          # 所有範例
│   ├── Basic/                         # 基礎範例 (01-40)
│   │   ├── Basic01_WithoutDI/
│   │   ├── Basic02_WithDI/
│   │   └── ...
│   ├── Intermediate/                  # 中階範例 (41-45)
│   │   ├── Intermediate01_ScopedFactory/
│   │   └── ...
│   └── Advanced/                      # 進階範例 (46-50)
│       ├── Advanced01_CustomServiceProvider/
│       └── ...
├── 快速開始.md                         # 5分鐘入門指南
├── ProjectTemplate.csproj             # 專案範本
└── Archive/                           # 備用文檔與工具
```

---

## 學習資源

### 官方文檔
- [Microsoft Docs - 依賴注入](https://learn.microsoft.com/zh-tw/dotnet/core/extensions/dependency-injection)
- [ASP.NET Core 中的依賴注入](https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/dependency-injection)

### 延伸閱讀
- SOLID 原則
- 設計模式
- 單元測試

---

## 快速開始

```bash
# 克隆專案
git clone <repository-url>

# 還原套件
dotnet restore

# 建置專案
dotnet build

# 執行第一個範例
cd Examples/Basic/Basic01_WithoutDI
dotnet run
```

---

## 授權

本專案採用 MIT 授權，歡迎自由使用和分享。

---

## 貢獻

歡迎提交 Issue 和 Pull Request！

---

**祝學習愉快！** 🎉
