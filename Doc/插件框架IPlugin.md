# ������ IPlugin

## ����

`IPlugin` �� DH.NCore �е�ͨ�ò���ӿڣ���� `PluginManager` ��������������Կ��ٹ���һ����ͨ�õĲ��ϵͳ��֧�ֲ�����֡����ء���ʼ������Դ�ͷŵ������������ڹ�����

**�����ռ�**��`NewLife.Model`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **�Զ�����**��ɨ������Զ����� `IPlugin` ʵ��
- **����ʶ��**��ͨ�� `PluginAttribute` ��ǲ����������
- **����ע��**��֧�ִ� `IServiceProvider` ʵ�������
- **��������**��֧�ֳ�ʼ�������ٻص�
- **��������**�������صķ���˳���ͷ���Դ

## ���ٿ�ʼ

### ������

```csharp
using NewLife.Model;

// ���ʵ��
[Plugin("MyApp")]  // ���֧�ֵ�����
public class MyPlugin : IPlugin
{
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        if (identity != "MyApp") return false;  // ��Ŀ������
        
        Console.WriteLine("MyPlugin ��ʼ���ɹ�");
        return true;
    }
}
```

### ���ز��

```csharp
using NewLife.Model;

// �������������
var manager = new PluginManager
{
    Identity = "MyApp",
    Provider = ObjectContainer.Provider
};

// ���ز���ʼ�����
manager.Load();
manager.Init();

// ʹ�ò��
foreach (var plugin in manager.Plugins)
{
    Console.WriteLine($"�Ѽ��ز��: {plugin.GetType().Name}");
}

// �ͷ���Դ
manager.Dispose();
```

## API �ο�

### IPlugin �ӿ�

```csharp
public interface IPlugin
{
    /// <summary>��ʼ��</summary>
    /// <param name="identity">���������ʶ</param>
    /// <param name="provider">�����ṩ��</param>
    /// <returns>���س�ʼ���Ƿ�ɹ�</returns>
    Boolean Init(String? identity, IServiceProvider provider);
}
```

**����˵��**��
- `identity`��������ʶ�����ڲ���ж��Ƿ�ΪĿ������
- `provider`�������ṩ�ߣ����ڻ�ȡ��������

**����ֵ**��
- `true`����ʼ���ɹ��������ò��
- `false`����Ŀ���������ʼ��ʧ�ܣ��Ƴ��ò��

### PluginAttribute ����

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PluginAttribute : Attribute
{
    public String Identity { get; set; }
}
```

���ڱ�ǲ��֧�ֵ�������ʶ��

**ʾ��**��
```csharp
// ֧�ֵ�������
[Plugin("WebServer")]
public class WebPlugin : IPlugin { }

// ֧�ֶ������
[Plugin("WebServer")]
[Plugin("ApiServer")]
public class MultiHostPlugin : IPlugin { }
```

### PluginManager ��

#### ����

```csharp
/// <summary>������ʶ</summary>
public String? Identity { get; set; }

/// <summary>���������ṩ��</summary>
public IServiceProvider? Provider { get; set; }

/// <summary>�������</summary>
public IPlugin[]? Plugins { get; set; }

/// <summary>��־�ṩ��</summary>
public ILog Log { get; set; }
```

#### Load - ���ز��

```csharp
public void Load()
```

ɨ�����г��򼯣�����ʵ�� `IPlugin` �ӿڵ����͡�

**���ع���**��
1. ɨ�������Ѽ��صĳ���
2. ����ʵ�� `IPlugin` �ķǳ�����
3. ��� `PluginAttribute`�����˷ǵ�ǰ�����Ĳ��
4. ͨ�������ṩ�߻���ʵ����

#### Init - ��ʼ�����

```csharp
public void Init()
```

���ε���ÿ������� `Init` �������Ƴ���ʼ��ʧ�ܵĲ����

#### LoadPlugins - ��ȡ�������

```csharp
public IEnumerable<Type> LoadPlugins()
```

����ȡ������ͣ���ʵ��������������Ҫ�Զ���ʵ�����߼��ĳ�����

## �����������

```
1. �������� PluginManager
2. ���� Load() - ���ֲ�ʵ�������
3. ���� Init() - ��ʼ�����
4. ���������...
5. ���� Dispose() - �������ٲ��
```

### ��������ʾ��

```csharp
public class LifecyclePlugin : IPlugin, IDisposable
{
    private ILogger? _logger;
    
    // ���캯�����������ز��ʱ����
    public LifecyclePlugin()
    {
        Console.WriteLine("1. ���캯��������");
    }
    
    // ��ʼ��������׼�����������
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        Console.WriteLine("2. Init ������");
        
        // ��ȡ����
        _logger = provider.GetService<ILogger>();
        _logger?.Info("�����ʼ��");
        
        return true;
    }
    
    // ���٣������ͷ�ʱ����
    public void Dispose()
    {
        Console.WriteLine("3. Dispose ������");
        _logger?.Info("�������");
    }
}
```

## ʹ�ó���

### 1. ������չ���

```csharp
// ������չ��ӿ�
public interface IDataProcessor
{
    String Name { get; }
    void Process(Object data);
}

// ���ʵ��
[Plugin("DataPipeline")]
public class JsonProcessor : IPlugin, IDataProcessor
{
    public String Name => "JSON������";
    
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        return identity == "DataPipeline";
    }
    
    public void Process(Object data)
    {
        var json = data.ToJson();
        Console.WriteLine($"����JSON: {json}");
    }
}

// ����ʹ��
var manager = new PluginManager { Identity = "DataPipeline" };
manager.Load();
manager.Init();

foreach (var plugin in manager.Plugins.OfType<IDataProcessor>())
{
    plugin.Process(myData);
}
```

### 2. �¼��������

```csharp
public interface IEventListener
{
    void OnEvent(String eventName, Object? args);
}

[Plugin("EventSystem")]
public class LoggingPlugin : IPlugin, IEventListener
{
    private ILog? _log;
    
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        _log = provider.GetService<ILog>();
        return identity == "EventSystem";
    }
    
    public void OnEvent(String eventName, Object? args)
    {
        _log?.Info($"�¼�: {eventName}, ����: {args}");
    }
}

// �¼��ַ�
public class EventDispatcher
{
    private readonly IEventListener[] _listeners;
    
    public EventDispatcher(PluginManager manager)
    {
        _listeners = manager.Plugins?.OfType<IEventListener>().ToArray() ?? [];
    }
    
    public void Dispatch(String eventName, Object? args = null)
    {
        foreach (var listener in _listeners)
        {
            listener.OnEvent(eventName, args);
        }
    }
}
```

### 3. ģ�黯Ӧ��

```csharp
// ģ��ӿ�
public interface IModule
{
    String Name { get; }
    void Configure(IObjectContainer container);
    void Start();
    void Stop();
}

// �û�ģ��
[Plugin("MainApp")]
public class UserModule : IPlugin, IModule, IDisposable
{
    public String Name => "�û�ģ��";
    
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        return identity == "MainApp";
    }
    
    public void Configure(IObjectContainer container)
    {
        container.AddTransient<IUserService, UserService>();
        container.AddTransient<IUserRepository, UserRepository>();
    }
    
    public void Start()
    {
        XTrace.WriteLine($"{Name} ������");
    }
    
    public void Stop()
    {
        XTrace.WriteLine($"{Name} ��ֹͣ");
    }
    
    public void Dispose() => Stop();
}

// ������
class Program
{
    static void Main()
    {
        var ioc = ObjectContainer.Current;
        
        var manager = new PluginManager
        {
            Identity = "MainApp",
            Provider = ioc.BuildServiceProvider()
        };
        
        manager.Load();
        manager.Init();
        
        // ����ģ��
        foreach (var module in manager.Plugins.OfType<IModule>())
        {
            module.Configure(ioc);
        }
        
        // ����ģ��
        foreach (var module in manager.Plugins.OfType<IModule>())
        {
            module.Start();
        }
        
        Console.WriteLine("��������˳�...");
        Console.ReadKey();
        
        manager.Dispose();
    }
}
```

### 4. ���Ŀ¼����

```csharp
public class PluginLoader
{
    public void LoadFromDirectory(String path)
    {
        if (!Directory.Exists(path)) return;
        
        // ���ز��Ŀ¼�µ����� DLL
        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            try
            {
                Assembly.LoadFrom(file);
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }
        }
    }
}

// ʹ��
var loader = new PluginLoader();
loader.LoadFromDirectory("plugins");

var manager = new PluginManager { Identity = "MyApp" };
manager.Load();  // �ᷢ���¼��صĳ����еĲ��
manager.Init();
```

## ���ʵ��

### 1. ����ӿ����

```csharp
// ������������չ��ӿ�
public interface IPlugin
{
    Boolean Init(String? identity, IServiceProvider provider);
}

// ���ܽӿ������ӿڷ���
public interface IDataExporter
{
    String Format { get; }
    Byte[] Export(Object data);
}

// ���ͬʱʵ�������ӿ�
[Plugin("ExportSystem")]
public class ExcelExporter : IPlugin, IDataExporter
{
    public String Format => "xlsx";
    
    public Boolean Init(String? identity, IServiceProvider provider) => true;
    
    public Byte[] Export(Object data) { /* ... */ }
}
```

### 2. ����ע��

```csharp
[Plugin("MyApp")]
public class DatabasePlugin : IPlugin
{
    private IDbConnection? _connection;
    private ILogger? _logger;
    
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        // ��������ȡ����
        _connection = provider.GetService<IDbConnection>();
        _logger = provider.GetService<ILogger>();
        
        if (_connection == null)
        {
            _logger?.Error("δ�ҵ����ݿ�����");
            return false;
        }
        
        return true;
    }
}
```

### 3. ������

```csharp
[Plugin("MyApp")]
public class SafePlugin : IPlugin
{
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        try
        {
            // ��ʼ���߼�
            return true;
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
            return false;  // ���� false �������׳��쳣
        }
    }
}
```

### 4. ��Դ�ͷ�

```csharp
[Plugin("MyApp")]
public class ResourcePlugin : IPlugin, IDisposable
{
    private FileStream? _file;
    private Boolean _disposed;
    
    public Boolean Init(String? identity, IServiceProvider provider)
    {
        _file = File.OpenWrite("plugin.log");
        return true;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _file?.Dispose();
    }
}
```

## �������

- [�������� ObjectContainer](object_container-��������ObjectContainer.md)
- [������չ Reflect](reflect-������չReflect.md)
- [������Ӧ������ Host](host-������Ӧ������Host.md)
