// Basic13: IOptions 強型別設定
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Basic13_IOptionsInjection;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "";
    public int Port { get; set; }
    public string Username { get; set; } = "";
}

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public void ShowSettings()
    {
        Console.WriteLine($"SMTP: {_settings.SmtpServer}");
        Console.WriteLine($"Port: {_settings.Port}");
        Console.WriteLine($"User: {_settings.Username}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic13: IOptions 注入 ===\n");

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EmailSettings:SmtpServer"] = "smtp.gmail.com",
                ["EmailSettings:Port"] = "587",
                ["EmailSettings:Username"] = "user@example.com"
            })
            .Build();

        var services = new ServiceCollection();
        services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
        services.AddTransient<EmailService>();

        var provider = services.BuildServiceProvider();
        var emailService = provider.GetRequiredService<EmailService>();

        emailService.ShowSettings();

        Console.WriteLine("\n✅ IOptions 提供強型別設定");
        Console.WriteLine("✅ 使用 Configure 方法註冊");
    }
}
