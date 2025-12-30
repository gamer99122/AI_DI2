// Advanced04: 插件系統
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Advanced04_PluginSystem;

// 插件介面
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void Execute();
}

// 插件1
public class LoggingPlugin : IPlugin
{
    public string Name => "日誌插件";
    public string Version => "1.0.0";

    public void Execute()
    {
        Console.WriteLine($"[{Name}] 記錄日誌...");
    }
}

// 插件2
public class CachePlugin : IPlugin
{
    public string Name => "快取插件";
    public string Version => "2.1.0";

    public void Execute()
    {
        Console.WriteLine($"[{Name}] 清理快取...");
    }
}

// 插件3
public class EmailPlugin : IPlugin
{
    public string Name => "郵件插件";
    public string Version => "1.5.0";

    public void Execute()
    {
        Console.WriteLine($"[{Name}] 發送郵件...");
    }
}

// 插件管理器
public class PluginManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Type> _pluginTypes = new();

    public PluginManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // 自動發現插件
    public void DiscoverPlugins()
    {
        Console.WriteLine("=== 發現插件 ===\n");

        var pluginType = typeof(IPlugin);
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && pluginType.IsAssignableFrom(t));

        foreach (var type in types)
        {
            _pluginTypes.Add(type);

            // 臨時創建實例以取得資訊
            var plugin = (IPlugin)Activator.CreateInstance(type)!;
            Console.WriteLine($"發現插件: {plugin.Name} v{plugin.Version}");
        }

        Console.WriteLine($"\n✅ 共發現 {_pluginTypes.Count} 個插件\n");
    }

    // 執行所有插件
    public void ExecuteAll()
    {
        Console.WriteLine("=== 執行所有插件 ===\n");

        foreach (var pluginType in _pluginTypes)
        {
            var plugin = (IPlugin)_serviceProvider.GetRequiredService(pluginType);
            plugin.Execute();
        }
    }

    // 執行特定插件
    public void ExecuteByName(string name)
    {
        var plugins = _serviceProvider.GetServices<IPlugin>();
        var plugin = plugins.FirstOrDefault(p => p.Name == name);

        if (plugin != null)
        {
            Console.WriteLine($"\n=== 執行插件: {name} ===");
            plugin.Execute();
        }
        else
        {
            Console.WriteLine($"插件 '{name}' 不存在");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Advanced04: 插件系統 ===\n");

        var services = new ServiceCollection();

        // 自動註冊所有插件
        var pluginType = typeof(IPlugin);
        var plugins = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && pluginType.IsAssignableFrom(t));

        foreach (var plugin in plugins)
        {
            services.AddTransient(typeof(IPlugin), plugin);
            services.AddTransient(plugin); // 也註冊具體類型
        }

        services.AddSingleton<PluginManager>();

        var provider = services.BuildServiceProvider();
        var manager = provider.GetRequiredService<PluginManager>();

        // 發現插件
        manager.DiscoverPlugins();

        // 執行所有插件
        manager.ExecuteAll();

        // 執行特定插件
        manager.ExecuteByName("快取插件");

        Console.WriteLine("\n✅ 插件系統特性:");
        Console.WriteLine("  - 自動發現插件");
        Console.WriteLine("  - 動態載入");
        Console.WriteLine("  - 熱插拔");
        Console.WriteLine("  - 版本管理");
    }
}
