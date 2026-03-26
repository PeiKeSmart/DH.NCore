# �������� ObjectContainer

## ���

`ObjectContainer` �� DH.NCore �е�����������������֧������ע�루DI�����ܡ��ṩ�򵥶�ǿ��� IoC �������ܣ�֧�ֵ�����˲̬���������������ڣ����� `IServiceProvider` �ӿڣ�������Ϊ ASP.NET Core ���� DI �Ĳ���������

**�����ռ�**��`NewLife.Model`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **������**�����ⲿ���������뾫��
- **������������**��������Singleton����˲̬��Transient����������Scoped��
- **���캯��ע��**���Զ��������캯������������ѡ��������Ŀ�ƥ�乹�캯��
- **����ί��**��֧���Զ���ʵ�������߼�
- **ȫ������**���ṩ `ObjectContainer.Current` �� `ObjectContainer.Provider` ȫ�ַ���
- **��׼����**��ʵ�� `IServiceProvider` �ӿڣ����׼ DI ����������
- **������֧��**��֧�� `IServiceScope` �� `IServiceScopeFactory`

## ���ٿ�ʼ

```csharp
using NewLife.Model;

// ��ȡȫ������
var ioc = ObjectContainer.Current;

// ע�����
ioc.AddSingleton<ILogger, ConsoleLogger>();       // ����
ioc.AddTransient<IUserService, UserService>();    // ˲̬
ioc.AddScoped<IDbContext, MyDbContext>();         // ������

// ���������ṩ��
var provider = ioc.BuildServiceProvider();

// ��������
var logger = provider.GetService<ILogger>();
var userService = provider.GetRequiredService<IUserService>();
```

## ��������

### IObjectContainer �ӿ�

���������ĺ��Ľӿڣ������˷���ע��ͽ����Ļ���������

| ��Ա | ���� | ˵�� |
|------|------|------|
| `Services` | `IList<IObject>` | ����ע�Ἧ�� |
| `Register` | ���� | ע�����ͺ����ƣ��ѱ�� EditorBrowsable.Never�� |
| `Add` | ���� | ���ӷ���ע�ᣬ�����ظ�����ͬһ������ |
| `TryAdd` | ���� | �������ӷ���ע�ᣬ�������ظ�����ͬһ������ |
| `GetService` | ���� | �������͵�ʵ�� |

### ObjectLifetime ö��

���������������ڲ��ԡ�

| ֵ | ˵�� |
|------|------|
| `Singleton` | ��ʵ��������Ӧ�ó�������������ֻ��һ��ʵ�� |
| `Scoped` | �����ڵ�ʵ����ͬһ�������ڹ���ʵ�� |
| `Transient` | ÿ��һ��ʵ����ÿ�����󶼴�����ʵ�� |

### IObject �ӿ�

����ӳ��ӿڣ�������������͡�ʵ�ֺ��������ڡ�

| ��Ա | ���� | ˵�� |
|------|------|------|
| `ServiceType` | `Type` | �������ͣ��ӿڻ���������� |
| `ImplementationType` | `Type?` | ʵ�����ͣ�����ʵ�������� |
| `Lifetime` | `ObjectLifetime` | �������ڣ�����ʵ���Ĵ��������ٲ��� |

### ServiceDescriptor ��

������������ʵ�� `IObject` �ӿڣ����������������Ϣ��

| ���� | ���� | ˵�� |
|------|------|------|
| `ServiceType` | `Type` | �������ͣ�ͨ���ǽӿڻ������ |
| `ImplementationType` | `Type?` | ʵ�����ͣ������ʵ���� |
| `Lifetime` | `ObjectLifetime` | �������� |
| `Instance` | `Object?` | ����ʵ����������ģʽ��Ч |
| `Factory` | `Func<IServiceProvider, Object>?` | ���󹤳������ڴ�������ʵ����ί�� |

## API �ο�

### ȫ�ַ���

#### Current

```csharp
public static IObjectContainer Current { get; set; }
```

ȫ��Ĭ������ʵ����Ӧ������ʱ�Զ�������

**ʾ��**��
```csharp
// ���κεط�����ȫ������
var container = ObjectContainer.Current;
container.AddSingleton<IMyService, MyService>();
```

#### Provider

```csharp
public static IServiceProvider Provider { get; set; }
```

ȫ��Ĭ�Ϸ����ṩ�ߣ��� `Current` �����Զ�������

**ʾ��**��
```csharp
// ֱ��ͨ��ȫ���ṩ�߽�������
var service = ObjectContainer.Provider.GetService<IMyService>();
```

#### SetInnerProvider

```csharp
// �����ڲ������ṩ�ߣ����� UseXxx �׶Σ�
public static void SetInnerProvider(IServiceProvider innerServiceProvider)

// �����ڲ������ṩ�߹��������� AddXxx �׶��ӳٰ󶨣�
public static void SetInnerProvider(Func<IServiceProvider> innerServiceProviderFactory)
```

������ ASP.NET Core �ȿ�ܵ� DI �������ɣ�ʵ����ʽ���ҡ��� ObjectContainer ���Ҳ�������ʱ�����Զ����ڲ��ṩ���в��ҡ�

**ʾ��**��
```csharp
// �� Startup.Configure ������
public void Configure(IApplicationBuilder app)
{
    ObjectContainer.SetInnerProvider(app.ApplicationServices);
}

// ���� Program.cs ��ʹ�ù����ӳٰ�
ObjectContainer.SetInnerProvider(() => app.Services);
```

### ����ע��

#### AddSingleton��������

```csharp
// ע������ӳ��
IObjectContainer AddSingleton<TService, TImplementation>()
IObjectContainer AddSingleton(Type serviceType, Type implementationType)

// ע��ʵ��
IObjectContainer AddSingleton<TService>(TService? instance = null)
IObjectContainer AddSingleton(Type serviceType, Object? instance)

// ע�Ṥ��
IObjectContainer AddSingleton<TService>(Func<IServiceProvider, TService> factory)
IObjectContainer AddSingleton(Type serviceType, Func<IServiceProvider, Object> factory)
```

ע�ᵥ����������Ӧ�ó�����������ֻ��һ��ʵ����

**ʾ��**��
```csharp
var ioc = ObjectContainer.Current;

// ����ӳ��
ioc.AddSingleton<ILogger, FileLogger>();

// ֱ��ע��ʵ��
var config = new AppConfig { Debug = true };
ioc.AddSingleton<AppConfig>(config);

// ����ί�У��ӳٴ�����
ioc.AddSingleton<IDbConnection>(sp =>
{
    var config = sp.GetService<AppConfig>();
    return new SqlConnection(config.ConnectionString);
});

// ע���������ͣ����贫��ʵ�����ڲ��Զ�ʵ������
ioc.AddSingleton<MyService>();
```

#### AddTransient��˲̬��

```csharp
IObjectContainer AddTransient<TService, TImplementation>()
IObjectContainer AddTransient<TService>()
IObjectContainer AddTransient(Type serviceType, Type implementationType)
IObjectContainer AddTransient(Type serviceType, Func<IServiceProvider, Object> factory)
```

ע��˲̬����ÿ�����󶼴�����ʵ����

**ʾ��**��
```csharp
// ÿ�ν�����������ʵ��
ioc.AddTransient<IUserService, UserService>();

// ����ע��
ioc.AddTransient<OrderProcessor>();

// ����ί��
ioc.AddTransient<HttpClient>(sp => new HttpClient
{
    BaseAddress = new Uri("https://api.example.com"),
    Timeout = TimeSpan.FromSeconds(30)
});
```

#### AddScoped��������

```csharp
IObjectContainer AddScoped<TService, TImplementation>()
IObjectContainer AddScoped<TService>()
IObjectContainer AddScoped(Type serviceType, Type implementationType)
IObjectContainer AddScoped(Type serviceType, Func<IServiceProvider, Object> factory)
```

ע�����������ͬһ�������ڹ���ʵ����

**ʾ��**��
```csharp
// ͬһ��������
ioc.AddScoped<IDbContext, AppDbContext>();

// ����ί��
ioc.AddScoped<IUnitOfWork>(sp =>
{
    var context = sp.GetService<IDbContext>();
    return new UnitOfWork(context);
});
```

#### TryAdd ϵ��

```csharp
// ����
IObjectContainer TryAddSingleton<TService, TImplementation>()
IObjectContainer TryAddSingleton<TService>(TService? instance = null)

// ������
IObjectContainer TryAddScoped<TService, TImplementation>()
IObjectContainer TryAddScoped<TService>(TService? instance = null)

// ˲̬
IObjectContainer TryAddTransient<TService, TImplementation>()
IObjectContainer TryAddTransient<TService>(TService? instance = null)
```

�������ӷ�������Ѵ�����ͬ�������������ӣ����� `false`��

**ʾ��**��
```csharp
// ����δע��ʱ����
ioc.TryAddSingleton<ILogger, ConsoleLogger>();

// �ڶ������Ӳ�����Ч
ioc.TryAddSingleton<ILogger, FileLogger>();  // ������

// �� Add ������ӣ����ע������ȣ�
ioc.AddSingleton<ILogger, FileLogger>();     // ������
```

### ���񹹽�

#### BuildServiceProvider

```csharp
public static IServiceProvider BuildServiceProvider(this IObjectContainer container)
public static IServiceProvider BuildServiceProvider(this IObjectContainer container, IServiceProvider? innerServiceProvider)
```

�Ӷ����������������ṩ�ߡ�

**ʾ��**��
```csharp
var ioc = ObjectContainer.Current;
ioc.AddSingleton<IMyService, MyService>();

// ��������
var provider = ioc.BuildServiceProvider();

// ���ڲ��ṩ�ߣ���ʽ���ң�
var provider2 = ioc.BuildServiceProvider(existingProvider);
```

#### BuildHost

```csharp
public static IHost BuildHost(this IObjectContainer container)
public static IHost BuildHost(this IObjectContainer container, IServiceProvider? innerServiceProvider)
```

�Ӷ�����������Ӧ��������

**ʾ��**��
```csharp
var ioc = ObjectContainer.Current;
ioc.AddSingleton<IMyService, MyService>();

var host = ioc.BuildHost();
host.Run();
```

### �������

#### GetService������������

```csharp
// IServiceProvider �ӿڷ���
Object? GetService(Type serviceType)

// ������չ����
T? GetService<T>(this IServiceProvider provider)
T? GetService<T>(this IObjectContainer container)
```

��ȡ����ʵ����δ�ҵ�ʱ���� null������ʱ���Ȳ������ע��ķ���

**ʾ��**��
```csharp
var service = provider.GetService(typeof(IMyService));
var typedService = provider.GetService<IMyService>();

// ֱ�Ӵ�������ȡ
var containerService = ioc.GetService<IMyService>();
```

#### GetRequiredService����Ҫ������

```csharp
Object GetRequiredService(this IServiceProvider provider, Type serviceType)
T GetRequiredService<T>(this IServiceProvider provider)
```

��ȡ��Ҫ�ķ���δ�ҵ�ʱ�׳� `InvalidOperationException` �쳣��

**ʾ��**��
```csharp
// ���δע����׳��쳣
var config = provider.GetRequiredService<AppConfig>();
```

#### GetServices������������

```csharp
IEnumerable<Object> GetServices(this IServiceProvider provider, Type serviceType)
IEnumerable<T> GetServices<T>(this IServiceProvider provider)
```

��ȡָ�����͵����з���ʵ����֧��ͬһ�������͵Ķ��ע�ᣩ������˳��Ϊע����������ע������ȷ��أ���

**ʾ��**��
```csharp
// ע����������
ioc.AddSingleton<IMessageHandler, EmailHandler>();
ioc.AddSingleton<IMessageHandler, SmsHandler>();
ioc.AddSingleton<IMessageHandler, PushHandler>();

// ��ȡ���д�����
var handlers = provider.GetServices<IMessageHandler>();
foreach (var handler in handlers)
{
    handler.Handle(message);
}
```

### ������֧��

#### IServiceScope �ӿ�

��Χ����ӿڣ��÷�Χ���������ڣ�ÿ����������ֻ��һ��ʵ����

```csharp
public interface IServiceScope : IDisposable
{
    IServiceProvider ServiceProvider { get; }
}
```

#### IServiceScopeFactory �ӿ�

��Χ���񹤳��ӿڡ�

```csharp
public interface IServiceScopeFactory
{
    IServiceScope CreateScope();
}
```

#### CreateScope

```csharp
IServiceScope? CreateScope(this IServiceProvider provider)
```

������Χ�����򣬸��������� Scoped ������ͬһʵ����

**ʾ��**��
```csharp
using var scope = provider.CreateScope();
var scopedService = scope.ServiceProvider.GetService<IDbContext>();
// scopedService �ڴ�����������Ψһ��

// ���������ٴλ�ȡ������ͬһʵ��
var scopedService2 = scope.ServiceProvider.GetService<IDbContext>();
// scopedService == scopedService2
```

#### CreateInstance

```csharp
Object? CreateInstance(this IServiceProvider provider, Type serviceType)
```

�����������ʹ�÷����ṩ������乹�캯�������������ڴ���δע�����͵�ʵ����

**ʾ��**��
```csharp
// ����δע�����͵�ʵ�����Զ�ע����ע�������
var instance = provider.CreateInstance(typeof(MyController));
```

## �����������

### Singleton��������

```csharp
ioc.AddSingleton<IMyService, MyService>();
```

- **����ʱ��**���״�����ʱ����
- **ʵ������**������Ӧ�ó���ֻ��һ��ʵ��
- **�ͷ�ʱ��**��Ӧ�ó������ʱ�ͷ�
- **���ó���**�����á���־���������״̬��ȫ�ֹ����ķ���
- **ע������**����������Ӧ�������������

### Transient��˲̬��

```csharp
ioc.AddTransient<IMyService, MyService>();
```

- **����ʱ��**��ÿ������ʱ����
- **ʵ������**��ÿ����������ʵ��
- **�ͷ�ʱ��**��ʹ���꼴�ɱ� GC ����
- **���ó���**������������״̬�ķ���
- **ע������**��Ƶ����������Ӱ������

### Scoped��������

```csharp
ioc.AddScoped<IMyService, MyService>();
```

- **����ʱ��**�����������״�����ʱ����
- **ʵ������**��ÿ��������һ��ʵ��
- **�ͷ�ʱ��**������������ʱ�ͷ�
- **���ó���**��Web �������ݿ������ĵ�
- **ע������**����Ҫͨ�� `CreateScope()` ����������

## ���캯��ע��

ObjectContainer ֧���Զ����캯��ע�룬ѡ��������Ŀ�ƥ�乹�캯����

```csharp
public interface ILogger { void Log(String message); }
public class ConsoleLogger : ILogger 
{
    public void Log(String message) => Console.WriteLine(message);
}

public interface IRepository { }
public class UserRepository : IRepository
{
    public ILogger Logger { get; }
    
    // ���캯��ע��
    public UserRepository(ILogger logger)
    {
        Logger = logger;
    }
}

public class UserService
{
    public IRepository Repository { get; }
    public ILogger Logger { get; }
    
    // ��������캯��
    public UserService(IRepository repository, ILogger logger)
    {
        Repository = repository;
        Logger = logger;
    }
}

// ע�����
var ioc = ObjectContainer.Current;
ioc.AddSingleton<ILogger, ConsoleLogger>();
ioc.AddSingleton<IRepository, UserRepository>();
ioc.AddSingleton<UserService>();

// ����ʱ�Զ�ע������
var provider = ioc.BuildServiceProvider();
var userService = provider.GetService<UserService>();
// userService.Logger �� userService.Repository �����Զ�ע��
```

### ���캯��ѡ�����

1. ��ȡ���͵����й���ʵ�����캯��
2. ������������������
3. ���γ���ƥ�䣬ѡ���һ�����в������ܽ����Ĺ��캯��
4. �������Ͳ�����Int32��String��Boolean �ȣ�ʹ��Ĭ��ֵ
5. ���û�п�ƥ��Ĺ��캯�����׳� `InvalidOperationException`

### ֧�ֵ�Ĭ�ϲ�������

| ���� | Ĭ��ֵ |
|------|--------|
| `Boolean` | `false` |
| `Char` | `(Char)0` |
| `SByte`/`Byte` | `0` |
| `Int16`/`UInt16` | `0` |
| `Int32`/`UInt32` | `0` |
| `Int64`/`UInt64` | `0` |
| `Single`/`Double` | `0` |
| `Decimal` | `0` |
| `DateTime` | `DateTime.MinValue` |
| `String` | `null` |

## �� ASP.NET Core ����

### ��ʽһ����Ϊ��������

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// ʹ�� ASP.NET Core �� DI
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// �����ڲ��ṩ�ߣ�ʵ����ʽ����
ObjectContainer.SetInnerProvider(app.Services);

// ���� ObjectContainer.Provider ���Խ��� ASP.NET Core ע��ķ���
var userService = ObjectContainer.Provider.GetService<IUserService>();
```

### ��ʽ�����ӳٰ�

```csharp
// �� AddXxx �׶�ʹ�ù����ӳٰ�
ObjectContainer.SetInnerProvider(() => app.Services);
```

### ��ʽ�������ʹ��

```csharp
// �� NewLife ������ע��
ObjectContainer.Current.AddSingleton<ICache, MemoryCache>();

// �� ASP.NET Core ��Ҳ���Է���
builder.Services.AddSingleton(sp => 
    ObjectContainer.Provider.GetService<ICache>()!);
```

## ʵսʾ��

### 1. ����̨Ӧ��

```csharp
public class Program
{
    public static void Main()
    {
        var ioc = ObjectContainer.Current;
        
        // ע�����
        ioc.AddSingleton<ILogger, ConsoleLogger>();
        ioc.AddSingleton<IConfiguration, JsonConfiguration>();
        ioc.AddTransient<IUserService, UserService>();
        
        // ����������
        var provider = ioc.BuildServiceProvider();
        var service = provider.GetRequiredService<IUserService>();
        service.Process();
    }
}
```

### 2. Web Ӧ��

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // �� NewLife ������ע��ͬ���� ASP.NET Core
        var ioc = ObjectContainer.Current;
        ioc.AddSingleton<IMyService, MyService>();
        
        // ���Ƶ� ASP.NET Core ����
        foreach (var item in ioc.Services)
        {
            // ת�������ӵ� services
        }
    }
}
```

### 3. ���ϵͳ

```csharp
public class PluginLoader
{
    private readonly IObjectContainer _container;
    
    public PluginLoader()
    {
        _container = new ObjectContainer();
    }
    
    public void LoadPlugins(String pluginPath)
    {
        var assemblies = Directory.GetFiles(pluginPath, "*.dll")
            .Select(Assembly.LoadFrom);
        
        foreach (var assembly in assemblies)
        {
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
            
            foreach (var type in pluginTypes)
            {
                _container.AddTransient(typeof(IPlugin), type);
            }
        }
    }
    
    public IEnumerable<IPlugin> GetPlugins()
    {
        var provider = _container.BuildServiceProvider();
        return provider.GetServices<IPlugin>();
    }
}
```

### 4. ��Ԫ����

```csharp
[TestClass]
public class UserServiceTests
{
    private IServiceProvider _provider;
    
    [TestInitialize]
    public void Setup()
    {
        var ioc = new ObjectContainer();
        
        // ע�� Mock ����
        ioc.AddSingleton<ILogger>(new MockLogger());
        ioc.AddSingleton<IRepository>(new MockRepository());
        ioc.AddTransient<IUserService, UserService>();
        
        _provider = ioc.BuildServiceProvider();
    }
    
    [TestMethod]
    public void TestCreateUser()
    {
        var service = _provider.GetRequiredService<IUserService>();
        var result = service.CreateUser("test");
        Assert.IsNotNull(result);
    }
}
```

## �߼��÷�

### �����滻

ʹ�� `Add` ���������ظ�ע��ͬһ�������ͣ�����ʱ�������ע��ģ�

```csharp
ioc.AddSingleton<ILogger, ConsoleLogger>();
ioc.AddSingleton<ILogger, FileLogger>();  // �滻

var logger = provider.GetService<ILogger>(); // ���� FileLogger
```

### ����ע��

ʹ�� `TryAdd` ʵ������ע�᣺

```csharp
// ����δע��ʱ����Ĭ��ʵ��
ioc.TryAddSingleton<ILogger, ConsoleLogger>();

// �û������ڴ�֮ǰע���Լ���ʵ��
```

### ����ģʽ

```csharp
ioc.AddSingleton<IConnectionFactory>(sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config["Database:ConnectionString"];
    return new SqlConnectionFactory(connStr);
});
```

### װ����ģʽ

```csharp
// ע�����ʵ��
ioc.AddSingleton<ILogger, ConsoleLogger>();

// ע��װ����
ioc.AddSingleton<ILogger>(sp =>
{
    var services = sp.GetServices<ILogger>().ToList();
    var inner = services.Count > 1 ? services[1] : services[0];
    return new LoggingDecorator(inner);
});
```

## ���ʵ��

### �Ƽ�

1. **����ʹ�ýӿ�ע��**�����ڲ��Ժ��滻ʵ��
2. **����ѡ����������**������������״̬����������������״̬����Ҫ�����ķ���
3. **ʹ�� TryAdd ��ֹ����**���������ʹ�� TryAdd �����û��Զ���ʵ��
4. **����ע�ᣬ�ӳٽ���**����Ӧ������ʱ�������ע��

### ����

1. **�������λ��ģʽ**����Ҫ��ҵ�������ֱ�ӵ��� `ObjectContainer.Provider`
2. **���ⵥ������������**����������Ӧ�������������
3. **����ѭ������**��A ���� B��B ���� A �ᵼ�½���ʧ��
4. **�������ȵ�·��ע��**������ע��Ӧ������ʱ���

### ��������ѡ����

```csharp
// ���á���־ �� ����
ioc.AddSingleton<AppConfig>();
ioc.AddSingleton<ILogger, FileLogger>();

// ���ݿ������� �� ������Web����˲̬������̨��
ioc.AddScoped<IDbContext, AppDbContext>();  // Web
ioc.AddTransient<IDbContext, AppDbContext>(); // ����̨

// ҵ�������� �� ˲̬
ioc.AddTransient<IValidator, UserValidator>();
```

## ��������

### Q: ��������ʱ�׳� "No suitable constructor" �쳣

**ԭ��**�����캯����ĳ����������δע�ᡣ

**���**��
```csharp
// ��鲢ע����������
ioc.AddSingleton<IDependency, Dependency>();
```

### Q: Scoped �����ڵ������޷���ȷ����

**ԭ��**������������������ڱ������򳤣�����й��ڵ�������ʵ����

**���**��
```csharp
// ���ù���ģʽ
ioc.AddSingleton<IServiceFactory>(sp => 
    new ServiceFactory(() => sp.CreateScope()));
```

### Q: ͬһ�ӿ�ע����ʵ�֣�ֻ�ܻ�ȡһ��

**ԭ��**��`GetService<T>()` ֻ�������ע���ʵ�֡�

**���**��
```csharp
// ʹ�� GetServices ��ȡ����ʵ��
var services = provider.GetServices<IHandler>();
```

### Q: ����� ASP.NET Core DI ����

**���**��ʹ�� `SetInnerProvider` ������ʽ���ң�
```csharp
ObjectContainer.SetInnerProvider(app.Services);
```

## �� Microsoft.Extensions.DependencyInjection �Ա�

| ���� | ObjectContainer | MS DI |
|------|-----------------|-------|
| ������С | ��С���������� | ��Ҫ����� |
| �������� | ֧�� | ֧�� |
| ���캯��ע�� | ֧�� | ֧�� |
| ����ע�� | ��֧�� | ��֧�� |
| ����ע�� | ��֧�� | ��֧�� |
| װ����ģʽ | �ֶ�֧�� | ֧�� |
| ��֤ | �� | ֧�� |
| ȫ�ַ��� | ���� | ��Ҫ����ʵ�� |

## �������

- [DH.NCore ��Ŀ��ҳ](https://github.com/PeiKeSmart/DH.NCore)
- 历史文档已归档，当前请以仓库内 Doc 为准
- [Ӧ������ IHost](./Ӧ������Host.md)
