// Basic12: IConfiguration 注入
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basic12_IConfigurationInjection;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail()
    {
        var smtpServer = _config["Email:SmtpServer"];
        var port = _config.GetValue<int>("Email:Port");
        Console.WriteLine($"SMTP: {smtpServer}:{port}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic12: IConfiguration 注入 ===\n");

        // 建立配置
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Email:SmtpServer"] = "smtp.gmail.com",
                ["Email:Port"] = "587"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddTransient<EmailService>();

        var provider = services.BuildServiceProvider();
        var emailService = provider.GetRequiredService<EmailService>();

        emailService.SendEmail();

        Console.WriteLine("\n✅ IConfiguration 通常註冊為 Singleton");
    }
}
