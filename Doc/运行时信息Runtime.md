# ����ʱ��Ϣ Runtime

## ����

`Runtime` �� DH.NCore �е�����ʱ��Ϣ�����࣬�ṩ��ǰ���л����ĸ��ּ��Ͳ������ܡ���������ϵͳ�жϡ�����������ȡ���ڴ������������Ϣ�ȹ��ܣ��ǿ�ƽ̨��������Ҫ���������

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **ƽ̨���**��Windows��Linux��OSX��Mono��Unity �����л���ʶ��
- **�����ж�**������̨��Web��������Ӧ�����ͼ��
- **�߾��ȼ�ʱ**����ƽ̨�� `TickCount64` ʵ�֣����� 32 λ���
- **�ڴ����**��GC ���պ͹������ͷ�
- **��������**�������ִ�Сд�Ļ���������ȡ

## ���ٿ�ʼ

```csharp
using NewLife;

// �жϲ���ϵͳ
if (Runtime.Windows)
    Console.WriteLine("������ Windows ϵͳ��");
else if (Runtime.Linux)
    Console.WriteLine("������ Linux ϵͳ��");

// �ж����л���
if (Runtime.IsConsole)
    Console.WriteLine("����̨Ӧ��");
if (Runtime.Container)
    Console.WriteLine("������������");

// ��ȡϵͳ����ʱ�䣨���룩
var uptime = Runtime.TickCount64;
Console.WriteLine($"ϵͳ������ {uptime / 1000 / 60} ����");

// ��ȡ��ǰ����ID
var pid = Runtime.ProcessId;
Console.WriteLine($"��ǰ����ID: {pid}");

// �ͷ��ڴ�
Runtime.FreeMemory();
```

## API �ο�

### ƽ̨���

#### Windows

```csharp
public static Boolean Windows { get; }
```

�Ƿ� Windows ����ϵͳ��

**ʵ�ַ�ʽ**��
- .NET Core/.NET 5+��ʹ�� `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`
- .NET Framework����� `Environment.OSVersion.Platform`

**ʾ��**��
```csharp
if (Runtime.Windows)
{
    // Windows ���еĲ���������� Win32 API
    Console.WriteLine("Windows �汾: " + Environment.OSVersion.VersionString);
}
```

#### Linux

```csharp
public static Boolean Linux { get; }
```

�Ƿ� Linux ����ϵͳ��

**ʾ��**��
```csharp
if (Runtime.Linux)
{
    // Linux ���еĲ��������ȡ /proc �ļ�ϵͳ
    var cpuInfo = File.ReadAllText("/proc/cpuinfo");
}
```

#### OSX

```csharp
public static Boolean OSX { get; }
```

�Ƿ� macOS ����ϵͳ��

#### Mono

```csharp
public static Boolean Mono { get; }
```

�Ƿ��� Mono ����ʱ���������С�ͨ����� `Mono.Runtime` �����Ƿ�������жϡ�

**Ӧ�ó���**��
- ĳЩ API �� Mono ����Ϊ��ͬ
- ��� Mono ���������Ż�����ݴ���

#### Unity

```csharp
public static Boolean Unity { get; }
```

�Ƿ��� Unity ���滷�������С�ͨ����� `UnityEngine.Application` �����Ƿ�������жϡ�

### �����ж�

#### IsConsole

```csharp
public static Boolean IsConsole { get; set; }
```

�Ƿ����̨Ӧ�ó���

**�ж��߼�**��
1. ���Է��� `Console.ForegroundColor` ��������̨�����Լ��
2. ��鵱ǰ�����Ƿ��������ھ��
3. �κ��쳣����Ϊ�ǿ���̨����

**ʾ��**��
```csharp
if (Runtime.IsConsole)
{
    Console.WriteLine("���ǿ���̨Ӧ�ã�����ʹ�ò�ɫ���");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("��ɫ�ı�");
    Console.ResetColor();
}
else
{
    // GUI Ӧ�û����
    Debug.WriteLine("�ǿ���̨����");
}
```

> **ע��**������ͨ������ `Runtime.IsConsole = false` ǿ�ƽ��ÿ���̨�жϡ�

#### Container

```csharp
public static Boolean Container { get; }
```

�Ƿ��� Docker/Kubernetes ���������С�ͨ����黷������ `DOTNET_RUNNING_IN_CONTAINER` ���жϡ�

**ʾ��**��
```csharp
if (Runtime.Container)
{
    // ���������µ����⴦��
    // ���磺ʹ�������ڵ�����·��
    var configPath = "/app/config";
}
```

#### IsWeb

```csharp
public static Boolean IsWeb { get; }
```

�Ƿ� Web Ӧ�ó���

**�ж��߼�**��
- .NET Core/.NET 5+������Ƿ������ `Microsoft.AspNetCore` ����
- .NET Framework����� `System.Web.HttpRuntime.AppDomainAppId` �Ƿ���ֵ

**ʾ��**��
```csharp
if (Runtime.IsWeb)
{
    // Web Ӧ�����еĴ���
    // ���磺ʹ�� HTTP ��������ع���
}
```

### ʱ�������

#### TickCount64

```csharp
public static Int64 TickCount64 { get; }
```

ϵͳ���������ĺ�������64λ�������ᷢ�� 32 λ������⡣

**ʵ�ַ�ʽ**��
- .NET Core 3.1+��ֱ��ʹ�� `Environment.TickCount64`
- Windows �ɿ�ܣ����� `GetTickCount64` Win32 API
- ����ƽ̨��ʹ�� `Stopwatch.GetTimestamp()` ���㣬����˵� `Environment.TickCount`

**Ӧ�ó���**��
- �߾��ȼ�ʱ
- ����ʱ����
- ���� `Environment.TickCount` Լ 49.7 �����������

**ʾ��**��
```csharp
// ����������ʱ
var start = Runtime.TickCount64;
DoSomeWork();
var elapsed = Runtime.TickCount64 - start;
Console.WriteLine($"��ʱ: {elapsed} ms");

// ���ó�ʱ
var timeout = Runtime.TickCount64 + 5000; // 5���ʱ
while (Runtime.TickCount64 < timeout)
{
    if (CheckCondition()) break;
    Thread.Sleep(100);
}
```

#### UtcNow

```csharp
public static DateTimeOffset UtcNow { get; }
```

��ȡ��ǰ UTC ʱ�䡣����ȫ��ʱ���ṩ�ߣ�`TimerScheduler.GlobalTimeProvider`�������ǳ�Ӧ���л����η�����ʱ��

**ʾ��**��
```csharp
var utcNow = Runtime.UtcNow;
Console.WriteLine($"UTCʱ��: {utcNow}");
Console.WriteLine($"����ʱ��: {utcNow.LocalDateTime}");
```

### ������Ϣ

#### ProcessId

```csharp
public static Int32 ProcessId { get; }
```

��ǰ���� ID��ʹ�û�������ظ���ȡ��

**ʵ�ַ�ʽ**��
- .NET 5+��ʹ�� `Environment.ProcessId`
- �ɿ�ܣ�ʹ�� `Process.GetCurrentProcess().Id`

**ʾ��**��
```csharp
Console.WriteLine($"��ǰ����ID: {Runtime.ProcessId}");

// ������־��¼
var logPrefix = $"[PID:{Runtime.ProcessId}]";
```

#### ClientId

```csharp
public static String ClientId { get; }
```

�ͻ��˱�ʶ����ʽΪ `ip@pid`�����ڷֲ�ʽϵͳ�б�ʶ�ͻ���ʵ����

**ʾ��**��
```csharp
Console.WriteLine($"�ͻ��˱�ʶ: {Runtime.ClientId}");
// �������: 192.168.1.100@12345

// ���ڷֲ�ʽ������Ϣ���������߱�ʶ��
var consumerId = Runtime.ClientId;
```

### ��������

#### GetEnvironmentVariable

```csharp
public static String? GetEnvironmentVariable(String variable)
```

��ȡ����������**�����ִ�Сд**��

**�ص�**��
- �ȳ��Ծ�ȷƥ��
- ��δ�ҵ����������л����������в����ִ�Сд�ıȽ�

**ʾ��**��
```csharp
// �����ִ�Сд��ȡ��������
var path = Runtime.GetEnvironmentVariable("PATH");
var home = Runtime.GetEnvironmentVariable("HOME");
var customVar = Runtime.GetEnvironmentVariable("MY_APP_CONFIG");
```

#### GetEnvironmentVariables

```csharp
public static IDictionary<String, String?> GetEnvironmentVariables()
```

��ȡ���л������������ز����ִ�Сд���ֵ䡣

**ʾ��**��
```csharp
var envVars = Runtime.GetEnvironmentVariables();
foreach (var kv in envVars.Where(e => e.Key.StartsWith("DOTNET")))
{
    Console.WriteLine($"{kv.Key} = {kv.Value}");
}
```

### ����

#### CreateConfigOnMissing

```csharp
public static Boolean CreateConfigOnMissing { get; set; }
```

�����ļ�������ʱ���Ƿ�����Ĭ�������ļ���Ĭ��Ϊ `true`��

**���÷�ʽ**��
- ����������`CreateConfigOnMissing=false`
- �������ã�`Runtime.CreateConfigOnMissing = false`

**ʾ��**��
```csharp
// ����������ֹ�Զ����������ļ�
Runtime.CreateConfigOnMissing = false;
```

### �ڴ����

#### FreeMemory

```csharp
public static Boolean FreeMemory(Int32 processId = 0, Boolean gc = true, Boolean workingSet = true)
```

�ͷ��ڴ档ִ�� GC ���ղ��ͷŹ�������Windows����

**����˵��**��
- `processId`��Ŀ�����ID��0 ��ʾ��ǰ����
- `gc`���Ƿ�ִ�� GC ���գ�����ǰ������Ч��
- `workingSet`���Ƿ��ͷŹ��������� Windows ��Ч��

**ִ�в���**��
1. ִ�� `GC.Collect` ������������
2. ���� `GC.WaitForPendingFinalizers` �ȴ��ս���
3. �ٴ�ִ�� `GC.Collect`
4. ���� `EmptyWorkingSet` �ͷŹ�������Windows��

**ʾ��**��
```csharp
// �����ͷ��ڴ�
var timer = new TimerX(state =>
{
    Runtime.FreeMemory();
}, null, 60_000, 60_000);  // ÿ����ִ��һ��

// ���ͷŹ�������������GC
Runtime.FreeMemory(gc: false);

// �ͷ�ָ�����̵��ڴ�
Runtime.FreeMemory(processId: 1234, gc: false);
```

> **ע��**��Ƶ������ `FreeMemory` ����Ӱ�����ܣ��������ڴ�ѹ���ϴ�ʱ���ڵ��á�

## ʹ�ó���

### 1. ��ƽ̨·������

```csharp
public String GetConfigPath()
{
    if (Runtime.Windows)
        return @"C:\ProgramData\MyApp\config.json";
    else if (Runtime.Linux)
        return "/etc/myapp/config.json";
    else if (Runtime.OSX)
        return "/Library/Application Support/MyApp/config.json";
    else
        return "config.json";
}
```

### 2. ������������

```csharp
public void ConfigureServices()
{
    if (Runtime.Container)
    {
        // �����������ӻ���������ȡ����
        var connStr = Runtime.GetEnvironmentVariable("DATABASE_URL");
        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(connStr));
    }
    else
    {
        // ���ؿ������������ļ���ȡ
        var connStr = Configuration.GetConnectionString("Default");
        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(connStr));
    }
}
```

### 3. �ڴ������ͷ�

```csharp
public class MemoryMonitor
{
    private readonly Timer _timer;
    private const Int64 MemoryThreshold = 500 * 1024 * 1024; // 500MB
    
    public MemoryMonitor()
    {
        _timer = new Timer(CheckMemory, null, 0, 30_000);
    }
    
    private void CheckMemory(Object? state)
    {
        var gcMemory = GC.GetTotalMemory(false);
        if (gcMemory > MemoryThreshold)
        {
            XTrace.WriteLine($"�ڴ泬����ֵ ({gcMemory / 1024 / 1024}MB)����ʼ�ͷ�");
            Runtime.FreeMemory();
        }
    }
}
```

### 4. ���ܼ�ʱ��

```csharp
public class PerformanceTimer : IDisposable
{
    private readonly String _operation;
    private readonly Int64 _startTime;
    
    public PerformanceTimer(String operation)
    {
        _operation = operation;
        _startTime = Runtime.TickCount64;
    }
    
    public void Dispose()
    {
        var elapsed = Runtime.TickCount64 - _startTime;
        XTrace.WriteLine($"{_operation} ��ʱ: {elapsed}ms");
    }
}

// ʹ��
using (new PerformanceTimer("���ݿ��ѯ"))
{
    var result = db.Query<User>().ToList();
}
```

## ���ʵ��

### 1. ƽ̨�ض��������

```csharp
// �Ƽ���ʹ�������жϸ���ƽ̨�ض�����
public void DoWork()
{
    if (Runtime.Windows)
        DoWorkWindows();
    else if (Runtime.Linux)
        DoWorkLinux();
    else
        DoWorkDefault();
}
```

### 2. ����Ƶ������ FreeMemory

```csharp
// ���Ƽ���ÿ�β������ͷ�
foreach (var item in items)
{
    ProcessItem(item);
    Runtime.FreeMemory();  // ����ɱ�֣�
}

// �Ƽ��������������ͷţ���ʱ�ͷ�
foreach (var item in items)
{
    ProcessItem(item);
}
Runtime.FreeMemory();  // ������ɺ�ͳһ�ͷ�
```

### 3. ʹ�� TickCount64 ���� DateTime ��ʱ

```csharp
// ���Ƽ���DateTime ��ʱ������ϵͳʱ�����Ӱ��
var start = DateTime.Now;
DoWork();
var elapsed = (DateTime.Now - start).TotalMilliseconds;

// �Ƽ���TickCount64 ����ϵͳʱ��Ӱ��
var start = Runtime.TickCount64;
DoWork();
var elapsed = Runtime.TickCount64 - start;
```

## �������

- [������Ϣ MachineInfo](machine_info-������ϢMachineInfo.md)
- [��־ϵͳ ILog](log-��־ILog.md)
- [�߼���ʱ�� TimerX](timerx-�߼���ʱ��TimerX.md)
