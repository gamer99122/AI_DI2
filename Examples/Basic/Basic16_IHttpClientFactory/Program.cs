// Basic16: IHttpClientFactory
using Microsoft.Extensions.DependencyInjection;

namespace Basic16_IHttpClientFactory;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task GetDataAsync()
    {
        var client = _httpClientFactory.CreateClient();
        Console.WriteLine("[ApiService] 使用 HttpClient");
        // var response = await client.GetAsync("https://api.example.com/data");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Basic16: IHttpClientFactory ===\n");

        var services = new ServiceCollection();
        services.AddHttpClient(); // 註冊 HttpClient 工廠
        services.AddTransient<ApiService>();

        var provider = services.BuildServiceProvider();
        var apiService = provider.GetRequiredService<ApiService>();

        await apiService.GetDataAsync();

        Console.WriteLine("\n✅ IHttpClientFactory 管理 HttpClient 生命週期");
        Console.WriteLine("✅ 避免 Socket 耗盡問題");
    }
}
