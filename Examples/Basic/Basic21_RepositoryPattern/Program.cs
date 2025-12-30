// Basic21: Repository 模式
using Microsoft.Extensions.DependencyInjection;

namespace Basic21_RepositoryPattern;

// 實體類別
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

// Repository 介面
public interface IUserRepository
{
    User? GetById(int id);
    IEnumerable<User> GetAll();
    void Add(User user);
    void Update(User user);
    void Delete(int id);
}

// Repository 實作
public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        Console.WriteLine($"[DB] 新增用戶: {user.Name} (ID: {user.Id})");
    }

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<User> GetAll() => _users;

    public void Update(User user)
    {
        var existing = GetById(user.Id);
        if (existing != null)
        {
            existing.Name = user.Name;
            Console.WriteLine($"[DB] 更新用戶: {user.Name}");
        }
    }

    public void Delete(int id)
    {
        var user = GetById(id);
        if (user != null)
        {
            _users.Remove(user);
            Console.WriteLine($"[DB] 刪除用戶 ID: {id}");
        }
    }
}

// Service 層
public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public void RegisterUser(string name)
    {
        var user = new User { Name = name };
        _repository.Add(user);
    }

    public void ListAllUsers()
    {
        var users = _repository.GetAll();
        Console.WriteLine($"\n所有用戶 ({users.Count()}):");
        foreach (var user in users)
        {
            Console.WriteLine($"  - ID:{user.Id}, Name:{user.Name}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Basic21: Repository 模式 ===\n");

        var services = new ServiceCollection();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserService>();

        var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();

            userService.RegisterUser("張三");
            userService.RegisterUser("李四");
            userService.RegisterUser("王五");

            userService.ListAllUsers();
        }

        Console.WriteLine("\n✅ Repository 封裝資料存取邏輯");
        Console.WriteLine("✅ Service 層不需知道資料如何儲存");
        Console.WriteLine("✅ 易於切換資料來源（記憶體 → SQL → NoSQL）");
    }
}
