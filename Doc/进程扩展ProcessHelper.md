# ������չ ProcessHelper

## ����

`ProcessHelper` �� DH.NCore �еĽ��̹��������࣬�ṩ���̲��ҡ����̿��ơ�������ִ�еȹ��ܡ�֧�� Windows �� Linux ˫ƽ̨���ܹ���ȷʶ�� dotnet/java �������̵���ʵ���ơ�

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ƽ̨֧��**��ͬʱ֧�� Windows �� Linux ϵͳ
- **��������ʶ��**����ȷʶ�� dotnet/java �йܽ��̵���ʵ������
- **��ȫ���̿���**���ṩ�º��˳���ǿ����ֹ���ַ�ʽ
- **������ִ��**��֧��ͬ��/�첽ִ�С�������񡢳�ʱ����

## ���ٿ�ʼ

```csharp
using NewLife;
using System.Diagnostics;

// ��ȡ������ʵ���ƣ�֧�� dotnet/java ������
var process = Process.GetCurrentProcess();
var name = process.GetProcessName();

// ִ�������ȡ���
var output = "ipconfig".Execute("/all");

// �����ش���ִ������
"notepad.exe".Run("test.txt", 0);

// ��ȫ�˳�����
process.SafetyKill();

// ǿ����ֹ������
process.ForceKill();
```

## API �ο�

### ���̲���

#### GetProcessName

```csharp
public static String GetProcessName(this Process process)
```

��ȡ���̵��߼����ơ����� dotnet/java �������̣�����������Ŀ�����/��� Jar ���ƣ�������չ������

**Ӧ�ó���**��
- ���̼�غ͹���
- ������
- ��־��¼��ʶ����ʵӦ����

**ʾ��**��
```csharp
// ��ͨ����
var notepad = Process.GetProcessesByName("notepad")[0];
notepad.GetProcessName()         // "notepad"

// dotnet �������̣����� MyApp.dll��
// �����У�dotnet /path/to/MyApp.dll --arg1 value1
var dotnetProcess = ...;
dotnetProcess.GetProcessName()   // "MyApp"

// java �������̣����� app.jar��
// �����У�java -jar /path/to/app.jar
var javaProcess = ...;
javaProcess.GetProcessName()     // "app"
```

#### GetCommandLine

```csharp
public static String? GetCommandLine(Int32 processId)
```

��ȡָ�����̵������������ַ�����

**ƽ̨ʵ��**��
- **Linux**����ȡ `/proc/{pid}/cmdline` �ļ�
- **Windows**��ͨ�� `NtQueryInformationProcess` ��ȡ���� PEB

**ʾ��**��
```csharp
var cmdLine = ProcessHelper.GetCommandLine(1234);
// Windows: "C:\Program Files\dotnet\dotnet.exe" MyApp.dll --env Production
// Linux: /usr/bin/dotnet MyApp.dll --env Production
```

#### GetCommandLineArgs

```csharp
public static String[]? GetCommandLineArgs(Int32 processId)
```

��ȡָ�����̵������в������顣

**ʾ��**��
```csharp
var args = ProcessHelper.GetCommandLineArgs(1234);
// ["dotnet", "MyApp.dll", "--env", "Production"]
```

### ���̿���

#### SafetyKill

```csharp
public static Process? SafetyKill(this Process process, Int32 msWait = 5_000, Int32 times = 50, Int32 interval = 200)
```

��ȫ�˳����̣��ºͷ�ʽ��������������ֹ�źţ��ý����л���ִ���������롣

**����˵��**��
- `msWait`�������źź�ĳ�ʼ�ȴ�ʱ�䣬Ĭ�� 5000 ����
- `times`����ѯ��������Ĭ�� 50 ��
- `interval`����ѯ�����Ĭ�� 200 ����

**ƽ̨ʵ��**��
- **Linux**������ `kill` �źţ�Ĭ�� SIGTERM��
- **Windows**��ִ�� `taskkill -pid {id}`

**ʾ��**��
```csharp
var process = Process.Start("MyApp.exe");

// �º͹رգ��ȴ���� 10 ��
process.SafetyKill(msWait: 10_000);

// ����Ƿ�ɹ��˳�
if (!process.GetHasExited())
{
    // ����δ�ڹ涨ʱ�����˳���������Ҫǿ����ֹ
    process.ForceKill();
}
```

#### ForceKill

```csharp
public static Process? ForceKill(this Process process, Int32 msWait = 5_000)
```

ǿ����ֹ�����������������ӽ��̡�

**ƽ̨ʵ��**��
- **Linux**������ `kill -9` �źţ�SIGKILL��
- **Windows**��ִ�� `taskkill /t /f /pid {id}`
- **.NET Core 3.0+**��ʹ�� `Process.Kill(true)` ��ֹ������

**ʾ��**��
```csharp
// ǿ����ֹ���̼��������ӽ���
process.ForceKill();

// ����ָ�������ĵȴ�ʱ��
process.ForceKill(msWait: 10_000);
```

#### GetHasExited

```csharp
public static Boolean GetHasExited(this Process process)
```

��ȫ��ȡ�����Ƿ�����ֹ�������̾�����ɷ���ʱ���� `true`����Ϊ���˳�����

**ʾ��**��
```csharp
if (process.GetHasExited())
{
    Console.WriteLine("�������˳�");
}
```

### ������ִ��

#### Run

```csharp
public static Int32 Run(
    this String cmd, 
    String? arguments = null, 
    Int32 msWait = 0, 
    Action<String?>? output = null, 
    Action<Process>? onExit = null, 
    String? working = null)
```

�����ش���ִ�������С�

**����˵��**��
- `cmd`����ִ���ļ�����·��
- `arguments`�������в���
- `msWait`���ȴ�ʱ�䣨0=���ȴ���<0=���޵ȴ���>0=��ȴ���������
- `output`������ص�ί�У��� msWait > 0��
- `onExit`�������˳��ص�
- `working`������Ŀ¼

**����ֵ**�������˳����룻δ�ȴ���ʱ���� -1

**ʾ��**��
```csharp
// ���ȴ�����ִ̨��
"notepad.exe".Run("test.txt");

// �ȴ�ִ����ɣ���ȡ�˳���
var exitCode = "ping".Run("localhost -n 4", 30_000);

// �������
var output = new StringBuilder();
"ipconfig".Run("/all", 5_000, line => output.AppendLine(line));
Console.WriteLine(output.ToString());

// ������Ŀ¼
"npm".Run("install", 60_000, working: @"C:\Projects\MyApp");

// �����˳�ʱ�ص�
"MyApp.exe".Run(onExit: p => Console.WriteLine($"�˳���: {p.ExitCode}"));
```

#### ShellExecute

```csharp
public static Process ShellExecute(
    this String fileName, 
    String? arguments = null, 
    String? workingDirectory = null)
```

�� Shell ��ִ�����Ŀ����̲��ǵ�ǰ���̵��ӽ��̣������汾�����˳���

**Ӧ�ó���**��
- ���ļ���ʹ��ϵͳĬ�ϳ���
- �� URL��ʹ��Ĭ���������
- ��������Ӧ�ó���

**ʾ��**��
```csharp
// ��Ĭ�ϳ�����ļ�
"document.pdf".ShellExecute();

// ��Ĭ�����������ַ
"https://github.com/PeiKeSmart/DH.NCore".ShellExecute();

// �Թ���Ա��������
// ע�⣺��Ҫ�� ProcessStartInfo ������ Verb = "runas"
"cmd.exe".ShellExecute("/k echo Hello");

// ָ������Ŀ¼
"MyApp.exe".ShellExecute("--config app.json", @"C:\Apps");
```

#### Execute

```csharp
public static String? Execute(
    this String cmd, 
    String? arguments = null, 
    Int32 msWait = 0, 
    Boolean returnError = false)
```

ִ��������ر�׼������ݡ�

**����˵��**��
- `msWait`���ȴ�ʱ�䣨0=����ֱ���˳���>0=��ʱ��ǿɱ��
- `returnError`���ޱ�׼���ʱ�Ƿ񷵻ش������

**ʾ��**��
```csharp
// ��ȡ IP ����
var ipConfig = "ipconfig".Execute("/all");

// ��ȡ Git �汾
var gitVersion = "git".Execute("--version");

// ִ�� Linux ����
var diskUsage = "df".Execute("-h");

// ��ʱ����
var result = "ping".Execute("localhost", 5_000);

// ʧ��ʱ���ش�����Ϣ
var output = "invalid_cmd".Execute(returnError: true);
```

## ʹ�ó���

### 1. �������

```csharp
public class ServiceManager
{
    public void StopService(String serviceName)
    {
        var processes = Process.GetProcesses()
            .Where(p => p.GetProcessName().EqualIgnoreCase(serviceName));
        
        foreach (var process in processes)
        {
            // �ȳ����º͹ر�
            process.SafetyKill(msWait: 10_000);
            
            // �����û�˳���ǿ����ֹ
            if (!process.GetHasExited())
            {
                process.ForceKill();
            }
        }
    }
}
```

### 2. �ű�ִ��

```csharp
public class ScriptRunner
{
    public String RunPowerShell(String script)
    {
        return "powershell".Execute($"-ExecutionPolicy Bypass -Command \"{script}\"", 60_000);
    }
    
    public String RunBash(String script)
    {
        return "bash".Execute($"-c \"{script}\"", 60_000);
    }
}
```

### 3. ���̼��

```csharp
public class ProcessMonitor
{
    public void Monitor(String appName)
    {
        while (true)
        {
            var processes = Process.GetProcesses()
                .Where(p => p.GetProcessName().EqualIgnoreCase(appName))
                .ToList();
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {appName} ������: {processes.Count}");
            
            foreach (var p in processes)
            {
                var cmdLine = ProcessHelper.GetCommandLine(p.Id);
                Console.WriteLine($"  PID={p.Id}, CommandLine={cmdLine}");
            }
            
            Thread.Sleep(5000);
        }
    }
}
```

## ���ʵ��

### 1. ���Źر�����

```csharp
// �Ƽ������ºͣ���ǿ��
public void StopProcess(Process process)
{
    // �º͹رգ������������Ļ���
    process.SafetyKill(msWait: 5_000);
    
    // �����û�˳���ǿ����ֹ
    if (!process.GetHasExited())
    {
        process.ForceKill();
    }
}
```

### 2. �������ó�ʱ

```csharp
// ���������ص����ó�ʱ
var quickResult = "echo".Execute("Hello", msWait: 1_000);      // ��������
var longResult = "npm".Execute("install", msWait: 300_000);   // ��ʱ����
```

### 3. ��������ص�

```csharp
// ʵʱ�����������
var lines = new List<String>();
"find".Run("/", 60_000, line =>
{
    if (!line.IsNullOrEmpty())
    {
        lines.Add(line);
        if (lines.Count % 1000 == 0)
            Console.WriteLine($"�Ѵ��� {lines.Count} ��");
    }
});
```

## ƽ̨����

| ���� | Windows | Linux |
|------|---------|-------|
| GetCommandLine | NtQueryInformationProcess | /proc/{pid}/cmdline |
| SafetyKill | taskkill | kill (SIGTERM) |
| ForceKill | taskkill /f /t | kill -9 (SIGKILL) |
| ShellExecute | Shell ִ�� | ��Ҫ���� UseShellExecute=true |

## ע������

1. **Ȩ��Ҫ��**�����ֲ���������Ҫ����Ա/root Ȩ��
2. **��������ֹ**��`ForceKill` ����ֹ�����ӽ��̣������ʹ��
3. **�����г���**��Windows �����г�������Լ 8191 �ַ�
4. **��������**��ִ������ʱע��������룬��ͨ�� `Encoding` ����ָ��

## �������

- [����ʱ��Ϣ Runtime](runtime-����ʱ��ϢRuntime.md)
- [������Ӧ������ Host](host-������Ӧ������Host.md)
- [������� DH.NAgent](https://github.com/PeiKeSmart)
