# ����ϵͳ Config

## ����

DH.NCore �ṩ��ǿ����������ϵͳ��֧�� XML��JSON��INI �ȶ��ָ�ʽ���Լ������ļ���Զ���������ġ�ͨ�� `Config<T>` ������Կ��ٴ���ǿ�������ã�֧���ȸ��º��Զ����档

**�����ռ�**��`NewLife.Configuration`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **ǿ��������**���̳� `Config<T>` �Զ����������ļ�
- **���ʽ֧��**��XML��JSON��INI��HTTP �������ṩ��
- **�ȸ���**�������ļ��仯ʱ�Զ�����
- **ע��֧��**��XML ��ʽ֧���Զ�����ע��
- **�ֲ�����**��֧�ֶ༶Ƕ�����ýṹ
- **��������**��֧��Զ���������ģ����ǳ���

## ���ٿ�ʼ

### ����������

```csharp
using NewLife.Configuration;
using System.ComponentModel;

/// <summary>Ӧ������</summary>
[Config("App")]  // �����ļ���Ϊ App.config
public class AppConfig : Config<AppConfig>
{
    [Description("Ӧ������")]
    public String Name { get; set; } = "MyApp";
    
    [Description("����˿�")]
    public Int32 Port { get; set; } = 8080;
    
    [Description("����ģʽ")]
    public Boolean Debug { get; set; }
    
    [Description("���ݿ�����")]
    public String ConnectionString { get; set; } = "Server=.;Database=test";
}
```

### ʹ������

```csharp
// ��ȡ���ã��Զ�����/���������ļ���
var config = AppConfig.Current;

Console.WriteLine($"Ӧ��: {config.Name}");
Console.WriteLine($"�˿�: {config.Port}");

// �޸Ĳ�����
config.Debug = true;
config.Save();
```

**�Զ����ɵ������ļ�** (`App.config`)��
```xml
<?xml version="1.0" encoding="utf-8"?>
<App>
  <!--Ӧ������-->
  <Name>MyApp</Name>
  <!--����˿�-->
  <Port>8080</Port>
  <!--����ģʽ-->
  <Debug>false</Debug>
  <!--���ݿ�����-->
  <ConnectionString>Server=.;Database=test</ConnectionString>
</App>
```

## API �ο�

### Config&lt;T&gt; ����

```csharp
public class Config<TConfig> where TConfig : Config<TConfig>, new()
{
    /// <summary>��ǰʹ�õ��ṩ��</summary>
    public static IConfigProvider? Provider { get; set; }
    
    /// <summary>��ǰʵ��</summary>
    public static TConfig Current { get; }
    
    /// <summary>��������</summary>
    public virtual Boolean Load();
    
    /// <summary>��������</summary>
    public virtual Boolean Save();
    
    /// <summary>���ü��غ󴥷�</summary>
    protected virtual void OnLoaded() { }
}
```

### ConfigAttribute ����

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : Attribute
{
    /// <summary>���������ļ�����������չ����</summary>
    public String Name { get; set; }
    
    /// <summary>�����ṩ������</summary>
    public Type? Provider { get; set; }
}
```

### IConfigProvider �ӿ�

```csharp
public interface IConfigProvider
{
    /// <summary>����</summary>
    String Name { get; set; }
    
    /// <summary>��Ԫ��</summary>
    IConfigSection Root { get; set; }
    
    /// <summary>�Ƿ�������</summary>
    Boolean IsNew { get; set; }
    
    /// <summary>��ȡ/��������ֵ</summary>
    String? this[String key] { get; set; }
    
    /// <summary>��������</summary>
    Boolean LoadAll();
    
    /// <summary>��������</summary>
    Boolean SaveAll();
    
    /// <summary>���ص�ģ��</summary>
    T? Load<T>(String? path = null) where T : new();
    
    /// <summary>��ģ�ͣ��ȸ��£�</summary>
    void Bind<T>(T model, Boolean autoReload = true, String? path = null);
    
    /// <summary>���øı��¼�</summary>
    event EventHandler? Changed;
}
```

## �����ṩ��

### XmlConfigProvider��Ĭ�ϣ�

```csharp
// �Զ�ʹ�� XML ��ʽ
[Config("Database")]
public class DbConfig : Config<DbConfig> { }

// ����ʽָ��
[Config("Database", Provider = typeof(XmlConfigProvider))]
public class DbConfig : Config<DbConfig> { }
```

### JsonConfigProvider

```csharp
[Config("appsettings", Provider = typeof(JsonConfigProvider))]
public class AppSettings : Config<AppSettings>
{
    public String Name { get; set; }
    public LoggingConfig Logging { get; set; } = new();
}

public class LoggingConfig
{
    public String Level { get; set; } = "Information";
    public Boolean Console { get; set; } = true;
}
```

**���ɵ� JSON �ļ�**��
```json
{
  "Name": "MyApp",
  "Logging": {
    "Level": "Information",
    "Console": true
  }
}
```

### InIConfigProvider

```csharp
[Config("settings", Provider = typeof(InIConfigProvider))]
public class IniConfig : Config<IniConfig>
{
    public String Server { get; set; }
    public Int32 Port { get; set; }
}
```

### HttpConfigProvider

��������Զ���������ģ�

```csharp
[HttpConfig("http://config.server.com", AppId = "myapp", Secret = "xxx")]
public class RemoteConfig : Config<RemoteConfig>
{
    public String Setting1 { get; set; }
}
```

## ʹ�ó���

### 1. ���ݿ�����

```csharp
[Config("Database")]
public class DatabaseConfig : Config<DatabaseConfig>
{
    [Description("���ݿ�����")]
    public String DbType { get; set; } = "MySql";
    
    [Description("�����ַ���")]
    public String ConnectionString { get; set; }
    
    [Description("���������")]
    public Int32 MaxPoolSize { get; set; } = 100;
    
    [Description("���ʱ���룩")]
    public Int32 CommandTimeout { get; set; } = 30;
}

// ʹ��
var db = DatabaseConfig.Current;
var connStr = db.ConnectionString;
```

### 2. Ƕ������

```csharp
[Config("Service")]
public class ServiceConfig : Config<ServiceConfig>
{
    public String Name { get; set; } = "MyService";
    public HttpConfig Http { get; set; } = new();
    public CacheConfig Cache { get; set; } = new();
}

public class HttpConfig
{
    public Int32 Port { get; set; } = 8080;
    public Int32 Timeout { get; set; } = 30;
    public Boolean Ssl { get; set; }
}

public class CacheConfig
{
    public String Type { get; set; } = "Memory";
    public Int32 Expire { get; set; } = 3600;
}
```

### 3. ��������

```csharp
[Config("Servers")]
public class ServersConfig : Config<ServersConfig>
{
    public String[] Hosts { get; set; } = ["localhost"];
    public List<EndpointConfig> Endpoints { get; set; } = new();
}

public class EndpointConfig
{
    public String Host { get; set; }
    public Int32 Port { get; set; }
}
```

### 4. ������֤

```csharp
[Config("App")]
public class AppConfig : Config<AppConfig>
{
    public Int32 Port { get; set; } = 8080;
    
    protected override void OnLoaded()
    {
        // ��֤����
        if (Port <= 0 || Port > 65535)
        {
            Port = 8080;
            XTrace.WriteLine("�˿�������Ч��ʹ��Ĭ��ֵ 8080");
        }
    }
}
```

### 5. ���ñ������

```csharp
var config = AppConfig.Current;
AppConfig.Provider.Changed += (s, e) =>
{
    XTrace.WriteLine("�����Ѹ���");
    // ���¶�ȡ����
    config = AppConfig.Current;
};
```

### 6. ֱ��ʹ���ṩ��

```csharp
// ���̳� Config<T>��ֱ��ʹ���ṩ��
var provider = new JsonConfigProvider { FileName = "custom.json" };
provider.LoadAll();

// ��ȡֵ
var name = provider["Name"];
var port = provider["Server:Port"].ToInt();

// ����ֵ
provider["Debug"] = "true";
provider.SaveAll();
```

## �����ļ�·��

Ĭ�������ļ������Ӧ�ó���Ŀ¼����ͨ�����·�ʽ�Զ��壺

```csharp
// ���·��
[Config("Config/App")]
public class AppConfig : Config<AppConfig> { }

// �����ṩ�߳�ʼ��
var provider = new XmlConfigProvider();
provider.Init("Config/App.config");
```

## ���ʵ��

### 1. ʹ�� Description ����

```csharp
[Description("��־����Debug/Info/Warn/Error")]
public String LogLevel { get; set; } = "Info";
```

### 2. �ṩĬ��ֵ

```csharp
public Int32 MaxRetry { get; set; } = 3;
public String[] AllowedHosts { get; set; } = ["*"];
```

### 3. ������Ϣ����

```csharp
[Config("Secrets")]
public class SecretsConfig : Config<SecretsConfig>
{
    // ����ӻ���������ȡ
    public String ApiKey { get; set; } = 
        Environment.GetEnvironmentVariable("API_KEY") ?? "";
}
```

### 4. ��������

```csharp
// ǿ�����¼�������
AppConfig._Current = null;
var fresh = AppConfig.Current;
```

## �� appsettings.json ����

```csharp
// ���� ASP.NET Core ��������
var provider = JsonConfigProvider.LoadAppSettings();
var connectionString = provider["ConnectionStrings:Default"];
var logLevel = provider["Logging:LogLevel:Default"];
```

## ���ü̳�

```csharp
public abstract class BaseConfig<T> : Config<T> where T : BaseConfig<T>, new()
{
    [Description("Ӧ�ð汾")]
    public String Version { get; set; } = "1.0.0";
    
    [Description("���õ���")]
    public Boolean Debug { get; set; }
}

[Config("MyApp")]
public class MyAppConfig : BaseConfig<MyAppConfig>
{
    [Description("�ض�����")]
    public String CustomSetting { get; set; }
}
```

## �������

- [JSON ���л�](json-JSON���л�.md)
- [XML ���л�](xml-XML���л�.md)
- [��־ϵͳ ILog](log-��־ILog.md)
