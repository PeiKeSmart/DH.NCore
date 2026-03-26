# ������Ӧ������ Host

## ����

`Host` �� DH.NCore �е�������Ӧ���������ṩӦ�ó����������ڹ������ܡ�֧���йܶ����̨�����Զ�����������ֹͣ�������˳��ȳ������ر��ʺϿ���̨Ӧ�á���̨����΢����ȳ�����

**�����ռ�**��`NewLife.Model`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **�����й�**��֧��ע��͹������ `IHostedService` ����
- **�������ڹ���**���Զ�����������ֹͣ���쳣�ع�
- **�����˳�**����Ӧ Ctrl+C��SIGINT��SIGTERM ��ϵͳ�ź�
- **��ƽ̨**��֧�� Windows��Linux��macOS
- **����ע��**���� `ObjectContainer` ��ȼ���
- **��ʱ����**��֧�������������ʱ��

## ���ٿ�ʼ

```csharp
using NewLife.Model;

// ��������
var host = new Host(ObjectContainer.Provider);

// ���Ӻ�̨����
host.Add<MyBackgroundService>();
host.Add<AnotherService>();

// ���У�����ֱ���յ��˳��źţ�
host.Run();
```

## API �ο�

### IHostedService �ӿ�

��̨�������ʵ�ִ˽ӿڣ�

```csharp
public interface IHostedService
{
    /// <summary>��ʼ����</summary>
    Task StartAsync(CancellationToken cancellationToken);
    
    /// <summary>ֹͣ����</summary>
    Task StopAsync(CancellationToken cancellationToken);
}
```

**ʵ��ʾ��**��
```csharp
public class MyBackgroundService : IHostedService
{
    private CancellationTokenSource? _cts;
    private Task? _task;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("MyBackgroundService ����");
        
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _task = ExecuteAsync(_cts.Token);
        
        return Task.CompletedTask;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("MyBackgroundService ֹͣ");
        
        _cts?.Cancel();
        
        if (_task != null)
            await Task.WhenAny(_task, Task.Delay(5000, cancellationToken));
    }
    
    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // ִ�к�̨����
            XTrace.WriteLine("��̨����ִ����...");
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

### Host ��

#### ���캯��

```csharp
public Host(IServiceProvider serviceProvider)
```

ͨ�������ṩ�ߴ�������ʵ����

**ʾ��**��
```csharp
// ʹ��ȫ������
var host = new Host(ObjectContainer.Provider);

// ʹ���Զ�������
var ioc = new ObjectContainer();
ioc.AddSingleton<ILogger, ConsoleLogger>();
var host = new Host(ioc.BuildServiceProvider());
```

#### Add - ���ӷ���

```csharp
// ���ӷ�������
void Add<TService>() where TService : class, IHostedService

// ���ӷ���ʵ��
void Add(IHostedService service)
```

**ʾ��**��
```csharp
var host = new Host(ObjectContainer.Provider);

// ͨ����������
host.Add<MyBackgroundService>();
host.Add<DataSyncService>();

// ͨ��ʵ������
var service = new CustomService(config);
host.Add(service);
```

#### Run / RunAsync - ��������

```csharp
// ͬ�����У�������
void Run()

// �첽����
Task RunAsync()
```

�����������������з���Ȼ�������ȴ��˳��źš�

**ʾ��**��
```csharp
// ͬ������
host.Run();

// �첽����
await host.RunAsync();

// �첽���к������������
_ = host.RunAsync();
// ��������...
```

#### StartAsync / StopAsync

```csharp
Task StartAsync(CancellationToken cancellationToken)
Task StopAsync(CancellationToken cancellationToken)
```

�ֶ�����������ֹͣ��

**ʾ��**��
```csharp
using var cts = new CancellationTokenSource();

// ��������
await host.StartAsync(cts.Token);

// ��һЩ����...
await Task.Delay(10000);

// �ֶ�ֹͣ
await host.StopAsync(cts.Token);
```

#### Close - �ر�����

```csharp
void Close(String? reason)
```

�����ر�����������ֹͣ���̡�

**ʾ��**��
```csharp
// ĳ�����������ر�
if (shouldShutdown)
{
    host.Close("���������ر�");
}
```

#### MaxTime ����

```csharp
public Int32 MaxTime { get; set; } = -1;
```

���ִ��ʱ�䣨���룩��Ĭ�� -1 ��ʾ�������С�

**ʾ��**��
```csharp
var host = new Host(ObjectContainer.Provider);
host.MaxTime = 60_000;  // �������60��
host.Add<MyService>();
host.Run();  // 60����Զ�ֹͣ
```

### ��̬����

#### RegisterExit - ע���˳��¼�

```csharp
// ���ܱ���ε���
static void RegisterExit(EventHandler onExit)

// ��ִ��һ��
static void RegisterExit(Action onExit)
```

ע��Ӧ���˳�ʱ�Ļص�������

**ʾ��**��
```csharp
// ע���˳���������
Host.RegisterExit(() =>
{
    XTrace.WriteLine("Ӧ�������˳���ִ������...");
    CleanupResources();
});

// �������Ļص�
Host.RegisterExit((sender, e) =>
{
    XTrace.WriteLine($"�յ��˳��ź�: {sender}");
});
```

## ��������

### AddHostedService ��չ����

```csharp
// ͨ������ע��
IObjectContainer AddHostedService<THostedService>()

// ͨ������ע��
IObjectContainer AddHostedService<THostedService>(
    Func<IServiceProvider, THostedService> factory)
```

**ʾ��**��
```csharp
var ioc = ObjectContainer.Current;

// ע���̨����
ioc.AddHostedService<MyBackgroundService>();
ioc.AddHostedService<DataSyncService>();

// ʹ�ù���
ioc.AddHostedService(sp =>
{
    var config = sp.GetRequiredService<AppConfig>();
    return new ConfigurableService(config);
});

// ��������������
var host = new Host(ioc.BuildServiceProvider());
host.Run();
```

## ʹ�ó���

### 1. �򵥺�̨����

```csharp
class Program
{
    static void Main()
    {
        var ioc = ObjectContainer.Current;
        ioc.AddHostedService<WorkerService>();
        
        var host = new Host(ioc.BuildServiceProvider());
        host.Run();
    }
}

public class WorkerService : IHostedService
{
    private Timer? _timer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, 0, 5000);
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }
    
    private void DoWork(Object? state)
    {
        XTrace.WriteLine($"������... {DateTime.Now}");
    }
}
```

### 2. �����Э��

```csharp
class Program
{
    static void Main()
    {
        var ioc = ObjectContainer.Current;
        
        // ע�Ṳ������
        ioc.AddSingleton<IMessageQueue, RedisMessageQueue>();
        ioc.AddSingleton<ILogger, FileLogger>();
        
        // ע������̨����
        ioc.AddHostedService<MessageConsumerService>();
        ioc.AddHostedService<HealthCheckService>();
        ioc.AddHostedService<MetricsCollectorService>();
        
        var host = new Host(ioc.BuildServiceProvider());
        host.Run();
    }
}
```

### 3. ��ʱ�������

```csharp
public class ScheduledTaskService : IHostedService
{
    private readonly ILogger _logger;
    private TimerX? _timer;
    
    public ScheduledTaskService(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ÿ���賿2��ִ��
        _timer = new TimerX(ExecuteTask, null, "0 0 2 * * *");
        _logger.Info("��ʱ�������������");
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _logger.Info("��ʱ���������ֹͣ");
        return Task.CompletedTask;
    }
    
    private void ExecuteTask(Object? state)
    {
        _logger.Info("ִ�ж�ʱ����...");
        // �����߼�
    }
}
```

### 4. ����ʱ�Ĳ�������

```csharp
class Program
{
    static async Task Main()
    {
        var ioc = ObjectContainer.Current;
        ioc.AddHostedService<TestService>();
        
        var host = new Host(ioc.BuildServiceProvider());
        host.MaxTime = 30_000;  // 30����Զ�ֹͣ
        
        await host.RunAsync();
        
        Console.WriteLine("�������");
    }
}
```

### 5. �����˳�����

```csharp
public class GracefulService : IHostedService
{
    private readonly List<Task> _runningTasks = new();
    private CancellationTokenSource? _cts;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        // ���������������
        for (var i = 0; i < 5; i++)
        {
            _runningTasks.Add(WorkerLoop(i, _cts.Token));
        }
        
        return Task.CompletedTask;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("�յ�ֹͣ�źţ��ȴ��������...");
        
        // ȡ����������
        _cts?.Cancel();
        
        // �ȴ�����������ɣ����ȴ�10��
        var timeout = Task.Delay(10_000, cancellationToken);
        var allTasks = Task.WhenAll(_runningTasks);
        
        await Task.WhenAny(allTasks, timeout);
        
        XTrace.WriteLine("����������ֹͣ");
    }
    
    private async Task WorkerLoop(Int32 id, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            XTrace.WriteLine($"Worker {id} ִ����...");
            await Task.Delay(1000, token).ConfigureAwait(false);
        }
    }
}
```

## �˳��źŴ���

Host �Զ����������˳��źţ�

| �ź� | ƽ̨ | ˵�� |
|------|------|------|
| Ctrl+C | ȫƽ̨ | ����̨�ж� |
| SIGINT | Linux/macOS | �ж��ź� |
| SIGTERM | Linux/macOS | ��ֹ�źţ�Docker Ĭ�ϣ� |
| SIGQUIT | Linux/macOS | �˳��ź� |
| ProcessExit | ȫƽ̨ | �����˳��¼� |

**Docker ����ע��**��
```dockerfile
# ʹ�� exec ��ʽ��ȷ���ź���ȷ����
CMD ["dotnet", "MyApp.dll"]

# ����ʹ�� tini ��Ϊ init ����
ENTRYPOINT ["/sbin/tini", "--"]
CMD ["dotnet", "MyApp.dll"]
```

## ���ʵ��

### 1. ��������˳��

����ע��˳��������������˳��ֹͣ��

```csharp
ioc.AddHostedService<DatabaseService>();  // ����������ֹͣ
ioc.AddHostedService<CacheService>();     // �ڶ�
ioc.AddHostedService<ApiService>();       // ����������ֹͣ
```

### 2. �쳣����

����ʧ��ʱ���Զ��ع��������ķ���

```csharp
public class MyService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // ����׳��쳣���������ķ���ᱻ�Զ�ֹͣ
        await InitializeAsync();
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // ȷ��ֹͣ�߼����׳��쳣
        try
        {
            return CleanupAsync();
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
            return Task.CompletedTask;
        }
    }
}
```

### 3. ��Դ�ͷ�

ʵ�� `IDisposable` ���ж���������

```csharp
public class ResourceService : IHostedService, IDisposable
{
    private FileStream? _file;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _file = File.OpenWrite("data.log");
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _file?.Dispose();
    }
}
```

## �������

- [�������� ObjectContainer](object_container-��������ObjectContainer.md)
- [�߼���ʱ�� TimerX](timerx-�߼���ʱ��TimerX.md)
- [��־ϵͳ ILog](log-��־ILog.md)
