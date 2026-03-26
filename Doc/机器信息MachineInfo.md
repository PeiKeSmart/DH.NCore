# ������ϢMachineInfo

## ����

`NewLife.MachineInfo` ���ڻ�ȡ������Ӳ����ϵͳ��Ϣ��֧��Windows��Linux��Mac�ȶ��ֲ���ϵͳ��

**��Ҫ����**��
- ��ȡ����ϵͳ��Ϣ�����ơ��汾��
- ��ȡӲ����Ϣ��CPU���ڴ桢���̣�
- ��ȡΨһ��ʶ��UUID��GUID�����кţ�
- ��ȡ��̬��Ϣ��CPUռ���ʡ��ڴ�ʹ�á������ٶȣ�

**�����ռ�**: `NewLife`  
**Դ��**: [DH.NCore/Common/MachineInfo.cs](https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Common/MachineInfo.cs)  
**文档**: 历史文档已归档，当前请以仓库内 Doc 为准

---

## ��������

### �����÷�

```csharp
using NewLife;

// ��ȡ��ǰ������Ϣ���״ε��û��ʼ����
var machine = MachineInfo.GetCurrent();

Console.WriteLine($"����ϵͳ��{machine.OSName} {machine.OSVersion}");
Console.WriteLine($"��������{machine.Processor}");
Console.WriteLine($"�ڴ�������{machine.Memory / 1024 / 1024 / 1024}GB");
Console.WriteLine($"Ӳ����ʶ��{machine.UUID}");
Console.WriteLine($"ϵͳ��ʶ��{machine.Guid}");
```

### �첽��ʼ��

```csharp
// �첽ע�������Ϣ���Ƽ���Ӧ������ʱ���ã�
var machine = await MachineInfo.RegisterAsync();

Console.WriteLine($"CPUռ�ã�{machine.CpuRate:P2}");
Console.WriteLine($"�����ڴ棺{machine.AvailableMemory / 1024 / 1024}MB");
```

### ˢ�¶�̬����

```csharp
var machine = MachineInfo.GetCurrent();

// ˢ�¶�̬���ݣ�CPU���ڴ桢����ȣ�
machine.Refresh();

Console.WriteLine($"CPUռ�ã�{machine.CpuRate:P2}");
Console.WriteLine($"�����ڴ棺{machine.FreeMemory / 1024 / 1024}MB");
Console.WriteLine($"�����ٶȣ�{machine.DownlinkSpeed / 1024}KB/s");
Console.WriteLine($"�ϴ��ٶȣ�{machine.UplinkSpeed / 1024}KB/s");
```

---

## ��������

### ��̬��Ϣ����ʼ���󲻱䣩

| ���� | ���� | ˵�� | ʾ�� |
|-----|------|------|------|
| `OSName` | String | ����ϵͳ���� | "Windows 11", "Ubuntu 22.04" |
| `OSVersion` | String | ϵͳ�汾�� | "10.0.22000", "5.15.0" |
| `Product` | String | ��Ʒ���� | "ThinkPad X1 Carbon" |
| `Vendor` | String | ������ | "Lenovo", "Dell" |
| `Processor` | String | �������ͺ� | "Intel Core i7-1165G7" |
| `UUID` | String | Ӳ��Ψһ��ʶ���������кţ� | "xxxx-xxxx-xxxx" |
| `Guid` | String | ����Ψһ��ʶ��ϵͳID�� | "xxxx-xxxx-xxxx" |
| `Serial` | String | ��������к� | "PF2ABCDE" |
| `Board` | String | ������Ϣ | "20XWCTO1WW" |
| `DiskID` | String | �������к� | "1234567890" |
| `Memory` | UInt64 | �ڴ��������ֽڣ� | 17179869184 (16GB) |

### ��̬��Ϣ����Ҫˢ�£�

| ���� | ���� | ˵�� |
|-----|------|------|
| `AvailableMemory` | UInt64 | �����ڴ棨�ֽڣ� |
| `FreeMemory` | UInt64 | �����ڴ棨�ֽڣ� |
| `CpuRate` | Double | CPUռ���ʣ�0-1�� |
| `UplinkSpeed` | UInt64 | ���������ٶȣ��ֽ�/�룩 |
| `DownlinkSpeed` | UInt64 | ���������ٶȣ��ֽ�/�룩 |
| `Temperature` | Double | �¶ȣ��ȣ� |
| `Battery` | Double | ���ʣ�ࣨ0-1�� |

---

## ���ķ���

### RegisterAsync - �첽ע��

```csharp
/// <summary>�첽ע��һ����ʼ����Ļ�����Ϣʵ��</summary>
public static Task<MachineInfo> RegisterAsync()
```

**�ص�**��
- �첽ִ�У����������߳�
- �״ε���ʱ��ʼ��������ֱ�ӷ��ػ�����
- �Զ����浽�ļ���`machine_info.json`�����ӿ���������ٶ�
- ע�ᵽ�������� `ObjectContainer`

**ʾ��**��
```csharp
// Ӧ������ʱ�첽ע��
await MachineInfo.RegisterAsync();

// ����ֱ��ʹ��
var machine = MachineInfo.Current;
```

### GetCurrent - ��ȡ��ǰʵ��

```csharp
/// <summary>��ȡ��ǰ��Ϣ�����δ������ȴ��첽ע����</summary>
public static MachineInfo GetCurrent()
```

**ʾ��**��
```csharp
var machine = MachineInfo.GetCurrent();
Console.WriteLine(machine.OSName);
```

### Refresh - ˢ�¶�̬����

```csharp
/// <summary>ˢ�¶�̬���ݣ�CPU���ڴ桢����ȣ�</summary>
public void Refresh()
```

**ʾ��**��
```csharp
var machine = MachineInfo.GetCurrent();
machine.Refresh();  // ����CPUռ�á��ڴ�ʹ�õ�

Console.WriteLine($"CPU: {machine.CpuRate:P}");
```

---

## Ψһ��ʶ˵��

### UUID��Ӳ����ʶ��

- **��Դ**���������к�
- **�ص�**����Ӳ���󶨣����������仯
- **ע��**������Ʒ�ƣ���ĳЩ���ƻ��������ظ�

```csharp
var uuid = machine.UUID;  // �� "A1B2C3D4-E5F6-..."
```

### Guid��ϵͳ��ʶ��

- **��Դ**��
  - Windows��ע��� `MachineGuid`
  - Linux��`/etc/machine-id`
  - Android��`android_id`
- **�ص�**�������ϵͳ��װ�󶨣���װϵͳ��仯
- **ע��**��Ghostϵͳ�����ظ�

```csharp
var guid = machine.Guid;  // �� "B1C2D3E4-F5A6-..."
```

### Serial�����кţ�

- **��Դ**����������кţ�BIOS��
- **�ص�**��Ʒ�ƻ����У���ʼǱ���ǩһ��
- **ע��**����װ��ͨ��Ϊ��

```csharp
var serial = machine.Serial;  // �� "PF2ABCDE"
```

### DiskID���������кţ�

- **��Դ**��ϵͳ�����к�
- **�ص�**�������Ӳ����
- **ע��**������Ӳ�̺�仯

---

## �ڴ���Ϣ���

### AvailableMemory�������ڴ棩

**�Ƽ�����Ӧ�����ұ����ͼ�ظ澯**

- **Linux**��`MemAvailable`���ں������ɰ�ȫ������ڴ棩
- **Windows**��`ullAvailPhys`����ǰ���������ڴ棩

```csharp
if (machine.AvailableMemory < 100 * 1024 * 1024)  // С��100MB
{
    Console.WriteLine("�ڴ治�㣬�ܾ�������");
}
```

### FreeMemory�������ڴ棩

**�ʺ����ڼ��չʾ���˹�����**

- **Linux**��`MemFree + Buffers + Cached + SReclaimable - Shmem`
- **Windows**���� `AvailableMemory` һ��

```csharp
Console.WriteLine($"�����ڴ棺{machine.FreeMemory / 1024 / 1024}MB");
```

---

## ʹ�ó���

### 1. Ӧ�ü��

```csharp
var timer = new TimerX(async _ =>
{
    var machine = MachineInfo.GetCurrent();
    machine.Refresh();
    
    // �ϱ��������
    await ReportMetrics(new
    {
        CpuRate = machine.CpuRate,
        AvailableMemory = machine.AvailableMemory,
        DownlinkSpeed = machine.DownlinkSpeed,
        UplinkSpeed = machine.UplinkSpeed
    });
}, null, 0, 60000);  // ÿ�����ϱ�
```

### 2. �豸ע��

```csharp
var machine = await MachineInfo.RegisterAsync();

var device = new Device
{
    UUID = machine.UUID,
    Guid = machine.Guid,
    OSName = machine.OSName,
    OSVersion = machine.OSVersion,
    Processor = machine.Processor,
    Memory = machine.Memory
};

await RegisterDevice(device);
```

### 3. ��Ȩ��֤

```csharp
var machine = MachineInfo.GetCurrent();

// ����Ӳ����ʶ��֤��Ȩ
if (!IsLicenseValid(machine.UUID))
{
    throw new UnauthorizedAccessException("δ��Ȩ���豸");
}
```

### 4. ����Ӧ��Դ����

```csharp
var machine = MachineInfo.GetCurrent();
var cpuCount = Environment.ProcessorCount;
var memoryGB = machine.Memory / 1024 / 1024 / 1024;

// ���ݻ������õ����̳߳ش�С
ThreadPool.SetMinThreads(cpuCount * 2, cpuCount * 2);

// �����ڴ��С������������
var cacheSize = (Int32)(memoryGB * 0.1 * 1024 * 1024 * 1024);  // 10%�ڴ�
```

### 5. ���ܸ澯

```csharp
var machine = MachineInfo.GetCurrent();
machine.Refresh();

if (machine.CpuRate > 0.9)
{
    SendAlert("CPUʹ���ʹ��ߣ�" + machine.CpuRate.ToString("P"));
}

if (machine.AvailableMemory < 100 * 1024 * 1024)
{
    SendAlert("�����ڴ治�㣺" + machine.AvailableMemory / 1024 / 1024 + "MB");
}
```

---

## ���ʵ��

### 1. Ӧ������ʱ�첽ע��

```csharp
class Program
{
    static async Task Main(String[] args)
    {
        // �첽ע�������Ϣ��������������
        _ = MachineInfo.RegisterAsync();
        
        // ����Ӧ�ó�ʼ��
        await StartApplication();
    }
}
```

### 2. ʹ�õ���ģʽ

```csharp
// MachineInfo �ڲ���ʵ�ֵ���
var machine = MachineInfo.Current;  // ʹ����ע���ʵ��
```

### 3. ����ˢ�¶�̬����

```csharp
// ��ҪƵ��ˢ�£�����������1��
var timer = new TimerX(_ =>
{
    MachineInfo.Current?.Refresh();
}, null, 0, 1000);
```

### 4. �����ļ�����

```csharp
// ������Ϣ���Զ����浽��
// - {Temp}/machine_info.json
// - {DataPath}/machine_info.json

// �´�����ʱ�Զ����ػ��棬�ӿ��ʼ���ٶ�
```

---

## ��չ����

### �Զ��������Ϣ�ṩ��

```csharp
public class CustomMachineInfo : IMachineInfo
{
    public void Init(MachineInfo info)
    {
        // �Զ����ʼ���߼�
        info["CustomField"] = "CustomValue";
    }
    
    public void Refresh(MachineInfo info)
    {
        // �Զ���ˢ���߼�
        info["Timestamp"] = DateTime.Now;
    }
}

// ע���Զ����ṩ��
MachineInfo.Provider = new CustomMachineInfo();
await MachineInfo.RegisterAsync();
```

### ʹ����չ����

```csharp
var machine = MachineInfo.GetCurrent();

// ������չ����
machine["AppVersion"] = "1.0.0";
machine["DeployTime"] = DateTime.Now;

// ��ȡ��չ����
var version = machine["AppVersion"] as String;
```

---

## ע������

### 1. �첽��ʼ��

```csharp
// �Ƽ����첽ע��
await MachineInfo.RegisterAsync();

// ���Ƽ���ͬ���ȴ�
var machine = MachineInfo.GetCurrent();  // ��������
```

### 2. Ȩ��Ҫ��

ĳЩ��Ϣ��Ҫ�ض�Ȩ�ޣ�
- **Windows**����ȡע�����Ҫ����ԱȨ�ޣ����ּ���
- **Linux**����ȡ `/sys` �� `/proc` ͨ����Ҫ root Ȩ��
- **����**������ͨ�û����У���ȡʧ��ʱʹ��Ĭ��ֵ

### 3. Ψһ��ʶ�����ظ�

- **UUID**�����ְ��ƻ�/����������ظ�
- **Guid**��Ghostϵͳ�����ظ�
- **����**����϶����ʶ����ΨһID

```csharp
var uniqueId = $"{machine.UUID}_{machine.Guid}_{machine.DiskID}".MD5();
```

### 4. ���ܿ���

- **��ʼ��**���״�ִ�н�����100-500ms��������ʹ�û���
- **ˢ��**��ÿ�ε��������ܿ����������Ƶ����
- **����**����ʱˢ�£���ÿ��һ�Σ�����ʵʱˢ��

---

## ��ƽ̨֧��

### Windows

֧�֣�
- ? OSName, OSVersion
- ? Processor, Memory
- ? UUID���������кţ�
- ? Guid��MachineGuid��
- ? Serial, Product, Vendor
- ? CpuRate, AvailableMemory
- ? UplinkSpeed, DownlinkSpeed

### Linux

֧�֣�
- ? OSName, OSVersion
- ? Processor, Memory
- ? UUID��DMI��
- ? Guid��/etc/machine-id��
- ? CpuRate, AvailableMemory
- ? UplinkSpeed, DownlinkSpeed
- ?? Serial, Product�������豸��֧�֣�

### macOS

֧�֣�
- ? OSName, OSVersion
- ? Processor, Memory
- ? UUID��Hardware UUID��
- ?? ������Ϣ֧������

---

## ��������

### 1. UUID Ϊʲô�ǿգ�

����ԭ��
- ���������
- ���ƻ�û���������к�
- Ȩ�޲���

�����ʹ�� `Guid` ����϶����ʶ��

### 2. Guid Ϊ `0-xxxx` ��ʽ��

��ʾ�޷���ȡϵͳ��ʶ���Զ����ɵ����GUID��

### 3. ˢ�º����ݲ��䣿

��飺
- �Ƿ���Ȩ�޶�ȡϵͳ��Ϣ
- ˢ�¼���Ƿ���̣������1�룩

### 4. ��λ�ȡ���������ٶȣ�

```csharp
var interfaces = NetworkInterface.GetAllNetworkInterfaces();
foreach (var ni in interfaces)
{
    var stats = ni.GetIPv4Statistics();
    Console.WriteLine($"{ni.Name}: {stats.BytesReceived} / {stats.BytesSent}");
}
```

---

## �ο�����

- 历史文档已归档，当前请以仓库内 Doc 为准
- **Դ��**: https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Common/MachineInfo.cs
- **����**: [setting-��������Setting.md](setting-��������Setting.md)

---

## ������־

- **2025-01**: �����ĵ���������ϸ˵��
- **2024**: ֧�� .NET 9.0���Ż���ƽ̨֧��
- **2023**: ���� AvailableMemory �� FreeMemory
- **2022**: ���������ٶȡ��¶ȡ���صȶ�̬��Ϣ
- **2020**: ��ʼ�汾��֧�ֻ���Ӳ����Ϣ��ȡ
