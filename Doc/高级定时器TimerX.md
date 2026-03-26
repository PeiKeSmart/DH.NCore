# �߼���ʱ��TimerX

## ����

`NewLife.Threading.TimerX` ��һ������ǿ��Ķ�ʱ��ʵ�֣����ϵͳ `System.Threading.Timer` �����������ƣ�

- **��������**���ص�ִ����Ϻ�ſ�ʼ��ʱ��һ��
- **֧���첽�ص�**��ԭ��֧�� `async/await`
- **����ʱ��ִ��**��֧�̶ֹ�ʱ��ִ�У���ÿ��2�㣩
- **Cron ����ʽ**��֧�ָ��ӵĶ�ʱ����
- **��·׷��**������ `ITracer` ���֧��
- **��ȫ�ͷ�**�������ھ�̬�������ϣ����ⱻGC��ǰ����

**�����ռ�**: `NewLife.Threading`  
**Դ��**: [DH.NCore/Threading/TimerX.cs](https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Threading/TimerX.cs)

---

## ��������

### �����÷���������ִ��

```csharp
using NewLife.Threading;

// 1����״�ִ�У�Ȼ��ÿ5��ִ��һ��
var timer = new TimerX(state =>
{
    Console.WriteLine($"ִ��ʱ�䣺{DateTime.Now}");
}, null, 1000, 5000);

// ʹ����ϼǵ��ͷ�
timer.Dispose();
```

**����˵��**��
- `state`���û����ݣ��ᴫ�ݸ��ص�����
- `dueTime`���ӳ�ʱ�䣨���룩���״�ִ��ǰ�ȴ���ʱ��
- `period`��������ڣ����룩����Ϊ0��-1��ʾִֻ��һ��

### �첽�ص�

```csharp
// ֧�� async/await ���첽�ص�
var timer = new TimerX(async state =>
{
    await Task.Delay(100);  // ģ���첽����
    Console.WriteLine("�첽�������");
}, null, 1000, 5000);
```

**�Զ�����**��
- ʹ�� `Func<Object, Task>` ʱ���Զ����� `IsAsyncTask = true` �� `Async = true`

### ����ʱ��ִ��

```csharp
// ÿ���賿2��ִ��
var start = DateTime.Today.AddHours(2);
var timer = new TimerX(state =>
{
    Console.WriteLine("ִ��������������");
}, null, start, 24 * 3600 * 1000);  // ����24Сʱ
```

**�ص�**��
- ��� `startTime` С�ڵ�ǰʱ�䣬���Զ��� `period` ֱ�����ڵ�ǰʱ��
- �Զ����� `Absolutely = true`
- ���� `SetNext` Ӱ�죬ʼ���ڹ̶�ʱ��ִ��

### Cron ����ʽ

```csharp
// ÿ������������9��ִ��
var timer = new TimerX(state =>
{
    Console.WriteLine("����������");
}, null, "0 0 9 * * 1-5");

// ֧�ֶ��Cron����ʽ���ֺŷָ�
var timer2 = new TimerX(state =>
{
    Console.WriteLine("�������");
}, null, "0 0 2 * * 1-5;0 0 3 * * 6");  // ������2�㣬����3��
```

**�Զ�����**��
- ʹ�� Cron ����ʽʱ���Զ����� `Absolutely = true`
- ��һ��ִ��ʱ���� Cron ����

---

## ��������

### 1. �����������

ϵͳ `Timer` �����⣺
```csharp
// System.Threading.Timer ������
var timer = new Timer(_ =>
{
    Thread.Sleep(3000);  // ���������ʱ3��
    Console.WriteLine("ִ��");
}, null, 0, 1000);  // ÿ1�봥��

// ���������ͬʱ�ж���ص���ִ�У���ɲ������⣡
```

TimerX �Ľ��������
```csharp
var timer = new TimerX(_ =>
{
    Thread.Sleep(3000);  // ��ʱ3��
    Console.WriteLine("ִ��");
}, null, 0, 1000);

// ִ�����̣�
// 0�룺��ʼ��1��ִ��
// 3�룺��1��ִ����ϣ��ȴ�1��
// 4�룺��ʼ��2��ִ��
// 7�룺��2��ִ����ϣ��ȴ�1��
// 8�룺��ʼ��3��ִ��
// ...
```

**ԭ��**���ص�ִ����Ϻ�ſ�ʼ������һ�ε��ӳ�ʱ�䣬ȷ��ͬһʱ��ֻ��һ���ص���ִ�С�

### 2. ���� TickCount �ľ�׼��ʱ

TimerX ʹ�� `Runtime.TickCount64` ��Ϊ��ʱ��׼���޾�ϵͳʱ��ز���

```csharp
// ��ʹ�ֶ�����ϵͳʱ�䣬TimerX Ҳ����Ӱ��
var timer = new TimerX(_ =>
{
    Console.WriteLine($"ִ�У�{DateTime.Now}");
}, null, 0, 5000);
```

**����**��
- ʹ�ÿ������������ϵͳʱ��
- ��������ʱ��ʱ������Ӱ��
- ÿ�� `SetNextTick` ��ˢ��ʱ���׼���Զ�����Ư��

### 3. ����ʱ���� Cron

- **`Absolutely = true`**����ʾ����ʱ��ִ�У����� `SetNext` Ӱ��
- **Cron ���캯��**���Զ����� `Absolutely = true`��ͨ�� `Cron.GetNext(now)` ������һ��ִ��ʱ��

```csharp
// ����ʱ�䶨ʱ��
var timer = new TimerX(_ => { }, null, DateTime.Today.AddHours(2), 24 * 3600 * 1000);
timer.Absolutely;  // true

// Cron ��ʱ��
var timer2 = new TimerX(_ => { }, null, "0 0 2 * * *");
timer2.Absolutely;  // true
timer2.Crons;       // Cron����
```

---

## ���캯�����

### 1. ��ͨ���ڶ�ʱ����ͬ����

```csharp
public TimerX(TimerCallback callback, Object? state, Int32 dueTime, Int32 period, String? scheduler = null)
```

**����**��
- `callback`: �ص�ί�� `void Callback(Object state)`
- `state`: �û�����
- `dueTime`: �ӳ�ʱ�䣨���룩���״�ִ��ǰ�ȴ�
- `period`: ������ڣ����룩��0��-1��ʾִֻ��һ��
- `scheduler`: ���������ƣ�Ĭ��ʹ�� `TimerScheduler.Default`

**ʾ��**��
```csharp
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
}, null, 1000, 5000);
```

### 2. �첽���ڶ�ʱ��

```csharp
public TimerX(Func<Object, Task> callback, Object? state, Int32 dueTime, Int32 period, String? scheduler = null)
```

**����**��
- `callback`: �첽�ص�ί�� `async Task Callback(Object state)`
- ��������ͬ��

**�Զ�����**��
- `IsAsyncTask = true`
- `Async = true`

**ʾ��**��
```csharp
var timer = new TimerX(async _ =>
{
    await DoWorkAsync();
}, null, 1000, 5000);
```

### 3. ����ʱ�䶨ʱ����ͬ����

```csharp
public TimerX(TimerCallback callback, Object? state, DateTime startTime, Int32 period, String? scheduler = null)
```

**����**��
- `startTime`: ���Կ�ʼʱ�䣬ָ��ʱ��ִ��
- `period`: ������ڣ����룩���������0

**�Զ�����**��
- `Absolutely = true`
- ��� `startTime` С�ڵ�ǰʱ�䣬�Զ��� `period` ����

**ʾ��**��
```csharp
// ÿ���賿2��ִ��
var start = DateTime.Today.AddHours(2);
var timer = new TimerX(_ =>
{
    Console.WriteLine("�賿����");
}, null, start, 24 * 3600 * 1000);
```

### 4. ����ʱ�䶨ʱ�����첽��

```csharp
public TimerX(Func<Object, Task> callback, Object? state, DateTime startTime, Int32 period, String? scheduler = null)
```

**�Զ�����**��
- `IsAsyncTask = true`
- `Async = true`
- `Absolutely = true`

### 5. Cron ��ʱ����ͬ����

```csharp
public TimerX(TimerCallback callback, Object? state, String cronExpression, String? scheduler = null)
```

**����**��
- `cronExpression`: Cron ����ʽ��֧�ַֺŷָ��������ʽ

**�Զ�����**��
- `Absolutely = true`
- ��������ʽ�����浽 `_crons` ����

**ʾ��**��
```csharp
// ÿ������������9��
var timer = new TimerX(_ =>
{
    Console.WriteLine("����������");
}, null, "0 0 9 * * 1-5");

// ���ʱ���
var timer2 = new TimerX(_ =>
{
    Console.WriteLine("�������");
}, null, "0 0 2 * * 1-5;0 0 3 * * 6");
```

### 6. Cron ��ʱ�����첽��

```csharp
public TimerX(Func<Object, Task> callback, Object? state, String cronExpression, String? scheduler = null)
```

**�Զ�����**��
- `IsAsyncTask = true`
- `Async = true`
- `Absolutely = true`

---

## ��������

| ���� | ���� | ˵�� |
|-----|------|------|
| `Id` | Int32 | ��ʱ��Ψһ��ʶ���Զ����� |
| `Period` | Int32 | ������ڣ����룩��0��-1��ʾִֻ��һ�� |
| `Async` | Boolean | �Ƿ��첽ִ�У�Ĭ�� false |
| `Absolutely` | Boolean | �Ƿ���Ծ�ȷʱ��ִ�У�Ĭ�� false |
| `Calling` | Boolean | �ص��Ƿ�����ִ�У�ֻ���� |
| `Timers` | Int32 | �ۼƵ��ô�����ֻ���� |
| `Cost` | Int32 | ƽ����ʱ�����룬ֻ���� |
| `NextTime` | DateTime | ��һ�ε���ʱ�䣨ֻ���� |
| `NextTick` | Int64 | ��һ��ִ��ʱ����������ֻ���� |
| `Crons` | Cron[] | Cron ����ʽ���ϣ�ֻ���� |
| `Tracer` | ITracer | ��·׷���� |
| `TracerName` | String | ��·׷�����ƣ�Ĭ�� `timer:{������}` |
| `State` | Object | �û����ݣ������ô洢 |
| `Scheduler` | TimerScheduler | ���������� |

---

## ���ķ���

### SetNext - ������һ��ִ��ʱ��

```csharp
/// <summary>������һ������ʱ��</summary>
/// <param name="ms">�ӳٺ�������С�ڵ���0��ʾ���ϵ���</param>
public void SetNext(Int32 ms)
```

**ʾ��**��
```csharp
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
    
    // ��̬������һ��ִ��ʱ��
    if (someCondition)
        timer.SetNext(10000);  // 10���ִ��
    else
        timer.SetNext(1000);   // 1���ִ��
}, null, 1000, 5000);
```

**ע��**��
- ���� `Absolutely = true` �Ķ�ʱ��������ʱ�䡢Cron����`SetNext` ��Ч
- ���ú�ỽ�ѵ������������

### Change - ���ļ�ʱ������

```csharp
/// <summary>���ļ�ʱ��������ʱ��ͷ�������֮���ʱ����</summary>
/// <param name="dueTime">�ӳ�ʱ��</param>
/// <param name="period">�������</param>
/// <returns>�Ƿ�ɹ�����</returns>
public Boolean Change(TimeSpan dueTime, TimeSpan period)
```

**ʾ��**��
```csharp
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
}, null, 1000, 5000);

// �޸�Ϊ2����״�ִ�У�Ȼ��ÿ10��һ��
timer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));
```

**����**��
- ���� `Absolutely = true` �Ķ�ʱ�������� false���޸�ʧ��
- ���� Cron ��ʱ�������� false���޸�ʧ��
- `period` Ϊ������ Infinite ʱ����ʱ���ᱻ����

### Dispose - ���ٶ�ʱ��

```csharp
/// <summary>���ٶ�ʱ��</summary>
public void Dispose()
```

**ʾ��**��
```csharp
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
}, null, 1000, 5000);

// ʹ������ͷ�
timer.Dispose();
```

**��Ҫ��**��
- TimerX �����ھ�̬������ `TimerScheduler` ��
- **�����ֶ����� `Dispose()`** ���ܴӵ������Ƴ�
- ���������ڴ�й©�Ͷ�ʱ������ִ��

---

## ��̬����

### Delay - �ӳ�ִ��

```csharp
/// <summary>�ӳ�ִ��һ��ί��</summary>
/// <param name="callback">�ص�ί��</param>
/// <param name="ms">�ӳٺ�����</param>
/// <returns>��ʱ��ʵ��</returns>
public static TimerX Delay(TimerCallback callback, Int32 ms)
```

**ʾ��**��
```csharp
// 10���ִ��һ�Σ�Ȼ������
var timer = TimerX.Delay(_ =>
{
    Console.WriteLine("�ӳ�����ִ��");
}, 10000);

// ע�⣺ί�п��ܻ�ûִ�У�timer����ͱ�GC������
// ���鱣��timer����
```

**ע��**��
- �Զ����� `Async = true`
- ��ִ��һ�Σ�Period=0��
- **����**����������� `timer` ���ã����ܱ� GC ���յ���δִ��

### Now - ����ĵ�ǰʱ��

```csharp
/// <summary>��ǰʱ�䡣��ʱ��ȡϵͳʱ�䣬����Ƶ����ȡ�������ƿ��</summary>
public static DateTime Now { get; }
```

**ʾ��**��
```csharp
var now = TimerX.Now;  // �������� DateTime.Now
Console.WriteLine(now);
```

**ԭ��**��
- ÿ500�������һ��ϵͳʱ��
- ����Ƶ������ `DateTime.Now` �������ƿ��
- �����ڶ�ʱ�侫��Ҫ�󲻸ߵĳ�������500ms��

---

## ������ TimerScheduler

TimerX ���� `TimerScheduler` ����ͳһ���ȡ�

### Ĭ�ϵ�����

```csharp
var timer = new TimerX(_ => { }, null, 1000, 5000);
timer.Scheduler;  // TimerScheduler.Default
```

### �Զ��������

```csharp
// ����ר��������
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
}, null, 1000, 5000, "MyScheduler");

timer.Scheduler.Name;  // "MyScheduler"
```

**���ó���**��
- ��Ҫ�����ĵ����̳߳�
- ���벻ͬҵ��Ķ�ʱ��
- ���Ʋ�ͬ�����������ȼ�

---

## ��·׷��

TimerX ������·׷��֧�֣��Զ�Ϊÿ��ִ�д��� Span��

### ����׷����

```csharp
using NewLife.Log;

var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ��");
}, null, 1000, 5000)
{
    Tracer = DefaultTracer.Instance,  // ����׷����
    TracerName = "MyTask"              // �Զ����������
};
```

### ׷������

ÿ�ζ�ʱ���������ᴴ��һ�� Span��
- **����**��Ĭ��Ϊ `timer:{������}`����ͨ�� `TracerName` �Զ���
- **��ǩ**����¼��ʱ��ID�����ڵ���Ϣ
- **��ʱ**����¼ÿ�λص�ִ�е�ʱ��

**�鿴ͳ��**��
```
Tracer[timer:DoWork] Total=100 Errors=0 Speed=0.02tps Cost=150ms MaxCost=200ms MinCost=100ms
```

�����[��·׷��ITracer](tracer-��·׷��ITracer.md)

---

## ʹ�ó���

### 1. ������������

```csharp
// ÿ���賿3��������������
var timer = new TimerX(state =>
{
    var days = (Int32)state!;
    var before = DateTime.Now.AddDays(-days);
    
    var count = Database.Delete("WHERE CreateTime < @time", new { time = before });
    Console.WriteLine($"������ {count} ����������");
}, 30, DateTime.Today.AddHours(3), 24 * 3600 * 1000);
```

### 2. �������

```csharp
var timer = new TimerX(_ =>
{
    foreach (var client in clients)
    {
        if (client.LastActive.AddMinutes(5) < DateTime.Now)
        {
            Console.WriteLine($"�ͻ��� {client.Id} ��ʱ���Ͽ�����");
            client.Disconnect();
        }
    }
}, null, 0, 60000);  // ÿ���Ӽ��һ��
```

### 3. ��������ͬ��

```csharp
var timer = new TimerX(async _ =>
{
    var data = await FetchDataFromApiAsync();
    await SaveToDatabase(data);
    Console.WriteLine("����ͬ�����");
}, null, 0, 300000);  // ÿ5����ͬ��һ��
```

### 4. �����ն�ʱ����

```csharp
// ÿ������������9�����ɱ���
var timer = new TimerX(_ =>
{
    var report = GenerateReport(DateTime.Today.AddDays(-1));
    SendEmail(report);
    Console.WriteLine("�����ѷ���");
}, null, "0 0 9 * * 1-5");
```

### 5. ���ڽ������

```csharp
var timer = new TimerX(_ =>
{
    var healthy = CheckSystemHealth();
    if (!healthy)
    {
        SendAlert("ϵͳ�쳣");
    }
}, null, 0, 30000);  // ÿ30����һ��
```

---

## ���ʵ��

### 1. �첽�ص�����

���ں�ʱ������ʹ���첽�ص�����������

```csharp
// �Ƽ�
var timer = new TimerX(async _ =>
{
    await DoHeavyWorkAsync();
}, null, 0, 5000);

// ���Ƽ�
var timer2 = new TimerX(_ =>
{
    DoHeavyWork();  // �����߳�
}, null, 0, 5000);
```

### 2. �����ͷ���Դ

```csharp
public class MyService : IDisposable
{
    private readonly TimerX _timer;
    
    public MyService()
    {
        _timer = new TimerX(_ => DoWork(), null, 0, 5000);
    }
    
    public void Dispose()
    {
        _timer?.Dispose();  // �ͷŶ�ʱ��
    }
}
```

### 3. ʹ�� Current ����

�ڻص��п��Է��ʵ�ǰ��ʱ����

```csharp
var timer = new TimerX(_ =>
{
    var current = TimerX.Current;
    Console.WriteLine($"��ʱ��[{current.Id}]��{current.Timers}��ִ��");
    
    // ��̬��������
    if (current.Timers > 10)
        current.Period = 10000;
}, null, 0, 5000);
```

### 4. ������

```csharp
var timer = new TimerX(_ =>
{
    try
    {
        DoWork();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"��ʱ�����쳣��{ex.Message}");
        // ��¼��־����Ҫ�׳��쳣
    }
}, null, 0, 5000);
```

### 5. Cron ���ӳ���

```csharp
// ����������9�㣬��������10��
var crons = "0 0 9 * * 1-5;0 0 10 * * 6";
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִ������");
}, null, crons);
```

---

## ע������

### 1. ���������ͷ�

```csharp
var timer = new TimerX(_ => { }, null, 0, 5000);
// ... ʹ�� ...
timer.Dispose();  // ������ã������ڴ�й©
```

### 2. Delay ����������

```csharp
// ����timer���ܱ�GC����
TimerX.Delay(_ => Console.WriteLine("ִ��"), 10000);

// ��ȷ����������
var timer = TimerX.Delay(_ => Console.WriteLine("ִ��"), 10000);
// ... ����timer���������� ...
```

### 3. ����ʱ�䲻���޸�

```csharp
var timer = new TimerX(_ => { }, null, DateTime.Today.AddHours(2), 86400000);
timer.SetNext(1000);  // ��Ч
timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));  // ���� false
```

### 4. State ��������

```csharp
var obj = new MyObject();
var timer = new TimerX(_ =>
{
    var state = _.State as MyObject;  // ����Ϊ null
}, obj, 0, 5000);

// ��� obj �� GC ���գ�State ���Ϊ null
```

### 5. ����Ϊ0ִֻ��һ��

```csharp
var timer = new TimerX(_ =>
{
    Console.WriteLine("ִֻ��һ��");
}, null, 1000, 0);  // Period=0��ִֻ��һ��
```

---

## ��������

### 1. ��ʱ����ִ�У�

��飺
- �Ƿ� GC ���գ��������ã�
- �Ƿ��� Dispose
- Period �Ƿ�Ϊ����
- Cron ����ʽ�Ƿ���ȷ

### 2. ��ʱ��ִ���˶�Σ�

TimerX �ǲ�������ģ��������������⡣������֣�����Ƿ񴴽��˶����ʱ��ʵ����

### 3. ���ֹͣ��ʱ����

```csharp
timer.Dispose();  // ���ٲ��Ƴ�
```

### 4. �����ͣ�ͻָ���

```csharp
// ��ͣ������Ϊִֻ��һ�Σ�
timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

// �ָ�
timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
```

### 5. Cron �;���ʱ�������

| ���� | Cron | ����ʱ�� |
|-----|------|---------|
| ����ʽ | ֧�ָ��ӹ��� | �̶�ʱ��+���� |
| ����� | �ߣ��뼶�����ڵȣ� | �ͣ��̶����ڣ� |
| ���� | �Եͣ�������� | �� |
| ���ó��� | ���Ӷ�ʱ | ������ |

---

## �ο�����

- **Cron �ĵ�**: [cron-Cron����ʽ.md](cron-Cron����ʽ.md)
- **��·׷��**: [tracer-��·׷��ITracer.md](tracer-��·׷��ITracer.md)
- 历史文档已归档，当前请以仓库内 Doc 为准
- **Դ��**: https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Threading/TimerX.cs

---

## ������־

- **2025-01**: �����ĵ���������ϸʾ�������ʵ��
- **2024**: ֧�� .NET 9.0���Ż�����
- **2023**: ���� Cron �����ʽ֧��
- **2022**: �����첽�ص�֧��
- **2020**: ��ʼ�汾������ TickCount �ľ�׼��ʱ
