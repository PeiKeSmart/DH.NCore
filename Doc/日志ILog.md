# ��־ILog

## ����

`NewLife.Log.ILog` �� DH.NCore �ĺ�����־�ӿڣ��ṩͳһ����־��¼�淶��当前源码仍沿用 `NewLife.Log` 命名空间��

ͨ����̬�� `XTrace` ���Է����ʹ����־���ܣ�֧�֣�
- �ļ���־��Ĭ�ϣ�
- ����̨��־
- ������־
- �Զ�����־ʵ��

**�����ռ�**: `NewLife.Log`  
**Դ��**: [DH.NCore/Log/ILog.cs](https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Log/ILog.cs)  
**文档**: 历史文档已归档，当前请以仓库内 Doc 为准

---

## ��������

### �����÷�

```csharp
using NewLife.Log;

// ��ʽ1��ʹ�� XTrace ��̬�ࣨ�Ƽ���
XTrace.WriteLine("����һ����Ϣ");
XTrace.WriteLine("�û�{0}��¼", "admin");

// ��ʽ2��ֱ��ʹ�� ILog �ӿ�
ILog log = XTrace.Log;
log.Info("����һ����Ϣ");
log.Error("����һ������");
```

### ��־����

```csharp
XTrace.Log.Debug("������Ϣ");      // ������־
XTrace.Log.Info("��ͨ��Ϣ");       // ��Ϣ��־
XTrace.Log.Warn("������Ϣ");       // ������־
XTrace.Log.Error("������Ϣ");      // ������־
XTrace.Log.Fatal("���ش���");      // ���ش�����־
```

### ����쳣

```csharp
try
{
    // ҵ�����
}
catch (Exception ex)
{
    XTrace.WriteException(ex);  // ����쳣��ջ
}
```

---

## ILog �ӿ�

```csharp
public interface ILog
{
    /// <summary>д��־</summary>
    void Write(LogLevel level, String format, params Object?[] args);

    /// <summary>������־</summary>
    void Debug(String format, params Object?[] args);

    /// <summary>��Ϣ��־</summary>
    void Info(String format, params Object?[] args);

    /// <summary>������־</summary>
    void Warn(String format, params Object?[] args);

    /// <summary>������־</summary>
    void Error(String format, params Object?[] args);

    /// <summary>���ش�����־</summary>
    void Fatal(String format, params Object?[] args);

    /// <summary>�Ƿ�������־��Ϊfalseʱ������κ���־</summary>
    Boolean Enable { get; set; }

    /// <summary>��־�ȼ���ֻ������ڵ��ڸü������־��Ĭ��Info</summary>
    LogLevel Level { get; set; }
}
```

### ��־���� LogLevel

```csharp
public enum LogLevel
{
    /// <summary>�ر���־</summary>
    Off = 0,
    
    /// <summary>���ش��󡣵���Ӧ�ó����˳�</summary>
    Fatal = 1,
    
    /// <summary>����Ӱ�칦�����У���Ҫ��������</summary>
    Error = 2,
    
    /// <summary>���档��Ӱ�칦�ܣ�����Ҫ��ע</summary>
    Warn = 3,
    
    /// <summary>��Ϣ��������־��Ϣ</summary>
    Info = 4,
    
    /// <summary>���ԡ�������־����������Ӧ�ر�</summary>
    Debug = 5,
    
    /// <summary>ȫ��</summary>
    All = 6
}
```

---

## XTrace ��̬��

`XTrace` ����־����Ҫʹ����ڣ��ṩ��ݵľ�̬������

### ��������

```csharp
// �����Ϣ��־
XTrace.WriteLine("��Ϣ");
XTrace.WriteLine("�û�{0}��{1}��¼", "admin", DateTime.Now);

// ����쳣
XTrace.WriteException(ex);
```

### �ؼ�����

```csharp
// ��ȡ��������־ʵ��
ILog log = XTrace.Log;  // Ĭ��Ϊ�ļ���־
XTrace.Log = new ConsoleLog();  // �л�Ϊ����̨��־

// �Ƿ����ģʽ
XTrace.Debug = true;  // �������ԣ����Debug������־

// ��־·��
XTrace.LogPath = "Logs";  // ������־�ļ���
```

---

## Ĭ���ļ���־

NewLife Ĭ��ʹ�� `TextFileLog`������־������ı��ļ���

### ����

- �Զ������ڷָ���־�ļ����� `2025-01-07.log`��
- �첽д�룬������ҵ���߳�
- �Զ����ݺ���������־
- ֧��������־·��������ļ���С��

### ����

�� `NewLife.config` �� `appsettings.json` �����ã�

```xml
<!-- NewLife.config -->
<Config>
  <Setting>
    <LogPath>Logs</LogPath>         <!-- ��־·�� -->
    <LogLevel>Info</LogLevel>        <!-- ��־���� -->
    <LogFileFormat>{0:yyyy-MM-dd}.log</LogFileFormat>  <!-- �ļ�������ʽ -->
  </Setting>
</Config>
```

```json
// appsettings.json
{
  "NewLife": {
    "Setting": {
      "LogPath": "Logs",
      "LogLevel": "Info",
      "LogFileFormat": "{0:yyyy-MM-dd}.log"
    }
  }
}
```

### ��־�ļ�ʾ��

```
2025-01-07 10:15:23.456  Info  Ӧ�ó�������
2025-01-07 10:15:24.123  Info  �û�admin��¼
2025-01-07 10:20:15.789  Warn  ���ӳ�����
2025-01-07 10:25:30.456  Error ���ݿ����ӳ�ʱ
System.TimeoutException: ���ӳ�ʱ
   at MyApp.Database.Query(String sql)
   at MyApp.Service.GetData()
```

---

## ����̨��־

�ڿ���̨Ӧ���У�����ʹ�� `UseConsole()` ����־���������̨��

### ʹ�÷���

```csharp
class Program
{
    static void Main(String[] args)
    {
        // �ض�����־������̨
        XTrace.UseConsole();
        
        XTrace.WriteLine("Ӧ�ó�������");
        XTrace.Log.Error("����һ������");
    }
}
```

### ��ɫ���

����̨��־֧�ֲ�ɫ�������ͬ��־����ʹ�ò�ͬ��ɫ��
- **Debug**: ��ɫ
- **Info**: ��ɫ
- **Warn**: ��ɫ
- **Error**: ��ɫ
- **Fatal**: ���ɫ

### ���̲߳�ɫ

```csharp
XTrace.UseConsole(useColor: true);  // ���ò�ɫ���

ThreadPool.QueueUserWorkItem(_ =>
{
    XTrace.WriteLine("�߳�1");  // �Զ�ʹ�ò�ͬ��ɫ
});

ThreadPool.QueueUserWorkItem(_ =>
{
    XTrace.WriteLine("�߳�2");  // �Զ�ʹ�ò�ͬ��ɫ
});
```

---

## ������־

����־ͨ�����緢�͵�Զ����־��������

### ʹ�÷���

```csharp
// ����������־
XTrace.Log = new NetworkLog("tcp://logserver:514");

XTrace.WriteLine("������־�ᷢ�͵�Զ�̷�����");
```

### ���ó���

- ����ʽ��־�ռ�
- �ֲ�ʽϵͳ��־�ۺ�
- ������Ӧ����־���

---

## ������־

ͬʱ��������Ŀ�ꡣ

### ʹ�÷���

```csharp
using NewLife.Log;

var compositeLog = new CompositeLog();
compositeLog.Add(new TextFileLog());    // �ļ���־
compositeLog.Add(new ConsoleLog());     // ����̨��־
compositeLog.Add(new NetworkLog("tcp://logserver:514"));  // ������־

XTrace.Log = compositeLog;
```

---

## �Զ�����־

ʵ�� `ILog` �ӿڴ����Զ�����־��

### ʾ�������ݿ���־

```csharp
public class DatabaseLog : ILog
{
    public Boolean Enable { get; set; } = true;
    public LogLevel Level { get; set; } = LogLevel.Info;

    public void Write(LogLevel level, String format, params Object?[] args)
    {
        if (!Enable || level > Level) return;

        var message = args.Length > 0 ? String.Format(format, args) : format;
        
        // д�����ݿ�
        Database.Insert("Logs", new
        {
            Level = level.ToString(),
            Message = message,
            CreateTime = DateTime.Now
        });
    }

    public void Debug(String format, params Object?[] args) => Write(LogLevel.Debug, format, args);
    public void Info(String format, params Object?[] args) => Write(LogLevel.Info, format, args);
    public void Warn(String format, params Object?[] args) => Write(LogLevel.Warn, format, args);
    public void Error(String format, params Object?[] args) => Write(LogLevel.Error, format, args);
    public void Fatal(String format, params Object?[] args) => Write(LogLevel.Fatal, format, args);
}

// ʹ��
XTrace.Log = new DatabaseLog();
```

---

## ʹ�ó���

### 1. Ӧ�ó���������־

```csharp
class Program
{
    static void Main(String[] args)
    {
        XTrace.WriteLine("Ӧ�ó�������");
        XTrace.WriteLine("�汾��{0}", Assembly.GetExecutingAssembly().GetName().Version);
        XTrace.WriteLine("����ʱ��{0}", Runtime.Version);
        
        try
        {
            RunApplication();
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
            XTrace.Log.Fatal("Ӧ�ó����쳣�˳�");
        }
    }
}
```

### 2. �ӿڵ�����־

```csharp
public class UserService
{
    public void Login(String username, String password)
    {
        XTrace.WriteLine("�û�{0}���Ե�¼", username);
        
        if (ValidateUser(username, password))
        {
            XTrace.WriteLine("�û�{0}��¼�ɹ�", username);
        }
        else
        {
            XTrace.Log.Warn("�û�{0}��¼ʧ�ܣ��������", username);
        }
    }
}
```

### 3. �쳣����

```csharp
try
{
    var data = await FetchDataAsync();
    ProcessData(data);
}
catch (TimeoutException ex)
{
    XTrace.Log.Warn("���ݻ�ȡ��ʱ��{0}", ex.Message);
}
catch (Exception ex)
{
    XTrace.WriteException(ex);
    throw;
}
```

### 4. ������־

```csharp
#if DEBUG
XTrace.Debug = true;  // ����������������
#endif

XTrace.Log.Debug("��ʼ�������ݣ�{0}��", data.Length);
foreach (var item in data)
{
    XTrace.Log.Debug("������Ŀ��{0}", item.Id);
    ProcessItem(item);
}
XTrace.Log.Debug("���ݴ������");
```

---

## ���ʵ��

### 1. ����ʹ����־����

```csharp
// Debug��������Ϣ����������Ӧ�ر�
XTrace.Log.Debug("����ֵ��{0}", value);

// Info��������Ϣ����¼��Ҫ����
XTrace.Log.Info("�û�{0}��¼", username);

// Warn��������Ϣ����Ӱ�칦�ܵ����ע
XTrace.Log.Warn("���ӳ�ʹ���ʣ�{0}%", usage);

// Error��������Ϣ��Ӱ�칦������
XTrace.Log.Error("���ݿ�����ʧ�ܣ�{0}", ex.Message);

// Fatal�����ش��󣬵���Ӧ���˳�
XTrace.Log.Fatal("�����ļ��𻵣�Ӧ�ó����˳�");
```

### 2. ������־��Ϣ����

```csharp
// ���Ƽ���ѭ���������־
foreach (var item in items)  // 100��������
{
    XTrace.Log.Debug("������{0}", item);  // ����100������־��
}

// �Ƽ����������
var count = 0;
foreach (var item in items)
{
    ProcessItem(item);
    count++;
}
XTrace.Log.Info("������ɣ�{0}��", count);
```

### 3. ʹ�ýṹ����־

```csharp
// �Ƽ���ʹ��ռλ��
XTrace.Log.Info("�û�{0}��{1}��¼��IP={2}", username, location, ip);

// ���Ƽ����ַ���ƴ��
XTrace.Log.Info("�û�" + username + "��" + location + "��¼��IP=" + ip);
```

### 4. ���ܿ���

```csharp
// �Ƽ������жϼ���
if (XTrace.Log.Enable && XTrace.Log.Level >= LogLevel.Debug)
{
    var expensiveData = GetExpensiveDebugInfo();  // �������
    XTrace.Log.Debug("������Ϣ��{0}", expensiveData);
}

// ���Ƽ���ֱ�ӵ���
XTrace.Log.Debug("������Ϣ��{0}", GetExpensiveDebugInfo());  // ��ʹDebug�ر�Ҳ��ִ��
```

---

## ���ù���

### ͨ����������

```csharp
// ������־����
XTrace.Log.Level = LogLevel.Warn;  // ֻ���Warn������

// �ر���־
XTrace.Log.Enable = false;

// ������־·��
XTrace.LogPath = "C:\\Logs";
```

### ͨ�������ļ�

```xml
<!-- NewLife.config -->
<Config>
  <Setting>
    <LogPath>Logs</LogPath>
    <LogLevel>Info</LogLevel>
    <Debug>false</Debug>
  </Setting>
</Config>
```

### ����ʱ�޸�

```csharp
// ��ʱ��������
var oldDebug = XTrace.Debug;
XTrace.Debug = true;

try
{
    DebugMethod();
}
finally
{
    XTrace.Debug = oldDebug;  // �ָ�����
}
```

---

## ȫ���쳣����

XTrace �Զ�����δ�����쳣��

```csharp
static XTrace()
{
    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
}
```

������δ�����쳣ʱ�����Զ�����쳣��־��

---

## ��������

### 1. ��ιر���־��

```csharp
// ��ʽ1���ر���־���
XTrace.Log.Enable = false;

// ��ʽ2��������־����ΪOff
XTrace.Log.Level = LogLevel.Off;

// ��ʽ3��ʹ�ÿ���־
XTrace.Log = Logger.Null;
```

### 2. ��־�ļ������

Ĭ����Ӧ�ó����Ŀ¼�� `Logs` �ļ����£��ļ�����ʽΪ `yyyy-MM-dd.log`��

### 3. �����������Ŀ�ꣿ

```csharp
var compositeLog = new CompositeLog();
compositeLog.Add(new TextFileLog());
compositeLog.Add(new ConsoleLog());
XTrace.Log = compositeLog;
```

### 4. ��־�ļ�̫����ô�죿

������־���ݺ��������ԣ�
```csharp
var textLog = new TextFileLog();
textLog.MaxBytes = 10 * 1024 * 1024;  // ���10MB
textLog.Backups = 10;                  // ����10������
```

### 5. �����ASP.NET Core��ʹ�ã�

```csharp
// Startup.cs �� Program.cs
public void Configure(IApplicationBuilder app)
{
    // ��־���Զ���ʼ����ֱ��ʹ��
    XTrace.WriteLine("Ӧ�ó�������");
}
```

---

## �ο�����

- 历史文档已归档，当前请以仓库内 Doc 为准
- **Դ��**: https://github.com/PeiKeSmart/DH.NCore/tree/master/DH.NCore/Log
- **��·׷��**: [tracer-��·׷��ITracer.md](tracer-��·׷��ITracer.md)

---

## ������־

- **2025-01**: �����ĵ���������ϸʾ��
- **2024**: ֧�� .NET 9.0
- **2023**: �Ż��첽д������
- **2022**: ����������־֧��
- **2020**: �ع���־�ܹ���ͳһ�ӿ�
