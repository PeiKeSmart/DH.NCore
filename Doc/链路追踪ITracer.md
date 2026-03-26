# ��·׷��ITracer

## ����

�ɹ۲����Ǻ����ִ�Ӧ�������ĺ���ָ��֮һ��DH.NCore �ṩ��һ����������·׷�ٹ淶��`ITracer`/`ISpan`������ʵ�������� APM��Application Performance Monitoring����

�봫ͳ APM ϵͳ��ͬ��NewLife ����·׷�ٲ���**���ز���+ͳ��**�ķ�ʽ��
- �ڱ���������ݲ����ͳ���ͳ��
- ���ϱ�ͳ�����ݺ�������������
- �����ʡ���紫��ʹ洢�ɱ�

NewLife ȫϵ����Ŀ��30+ ������������� `ITracer` ��㣬�����߿��ԣ�
1. ������ػ�ȡ�������ָ��
2. �����ǳ����ƽ̨ʵ�ֲַ�ʽ׷��
3. ���ڹ淶��չ�Զ������

**Nuget ��**: `DH.NCore`  
**Դ��**: [DH.NCore/Log/ITracer.cs](https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Log/ITracer.cs)

---

## ��������

### �����÷�

��򵥵����ʾ����

```csharp
using var span = tracer?.NewSpan("��������");
```

ʹ�� `using` �ؼ���ȷ�� span �����������ʱ�Զ���ɣ���¼��ʱ�ʹ�����

### ����ʾ��

����������������ݵ����ʾ����

```csharp
private void Ss_Received(Object? sender, ReceivedEventArgs e)
{
    var ns = (this as INetSession).Host;
    var tracer = ns?.Tracer;
    
    // ������㣬��¼���ղ���
    using var span = tracer?.NewSpan($"net:{ns?.Name}:Receive", e.Message);
    try
    {
        OnReceive(e);
    }
    catch (Exception ex)
    {
        // ��Ǵ��󲢼�¼�쳣��Ϣ
        span?.SetError(ex, e.Message ?? e.Packet);
        throw;
    }
}
```

���ʾ����ʾ�ˣ�
- **�������**��ʹ�ö�̬���ɵ�����
- **���ݱ�ǩ**���ڶ������� `e.Message` ��Ϊ���ݱ�ǩ
- **�쳣����**��ͨ�� `SetError` ����쳣���
- **�Զ���¼**��`using` ����Զ���¼��ʱ

ͨ�������㣬���ǿ��ԣ�
- ͳ�ƽ������ݰ��Ĵ���
- ��¼ÿ�ν��յĺ�ʱ
- ͳ���쳣��������
- �鿴�������ݺͱ�ǩ

---

## ���Ľӿ�

### ITracer �ӿ�

���ܸ������������� APM �淶�ĺ��Ľӿڡ�

```csharp
public interface ITracer
{
    #region ����
    /// <summary>�������ڡ���λ�룬Ĭ��15��</summary>
    Int32 Period { get; set; }

    /// <summary>������������������������ڣ����ֻ��¼ָ�������������¼���Ĭ��1</summary>
    Int32 MaxSamples { get; set; }

    /// <summary>����쳣�����������������ڣ����ֻ��¼ָ���������쳣�¼���Ĭ��10</summary>
    Int32 MaxErrors { get; set; }

    /// <summary>��ʱʱ�䡣������ʱ��ʱǿ�Ʋ�������λ���룬Ĭ��5000</summary>
    Int32 Timeout { get; set; }

    /// <summary>����ǩ���ȡ������ó���ʱ���ضϣ�Ĭ��1024�ַ�</summary>
    Int32 MaxTagLength { get; set; }

    /// <summary>��http/rpc����ע��TraceId�Ĳ�������Ϊ�ձ�ʾ��ע�룬Ĭ��W3C��׼��traceparent</summary>
    String? AttachParameter { get; set; }
    #endregion

    /// <summary>����Span������</summary>
    /// <param name="name">�������ƣ����ڱ�ʶ��ͬ���������</param>
    ISpanBuilder BuildSpan(String name);

    /// <summary>��ʼһ��Span</summary>
    /// <param name="name">�������ƣ����ڱ�ʶ��ͬ���������</param>
    ISpan NewSpan(String name);

    /// <summary>��ʼһ��Span��ָ�����ݱ�ǩ</summary>
    /// <param name="name">�������ƣ����ڱ�ʶ��ͬ���������</param>
    /// <param name="tag">���ݱ�ǩ����¼�ؼ�������Ϣ</param>
    ISpan NewSpan(String name, Object? tag);

    /// <summary>�ض�����Span���������ݣ����ü���</summary>
    ISpanBuilder[] TakeAll();
}
```

#### �ؼ�����˵��

| ���� | Ĭ��ֵ | ˵�� |
|-----|-------|------|
| `Period` | 15�� | �������ڣ������ϱ����ݵ�ʱ���� |
| `MaxSamples` | 1 | ÿ�������ڼ�¼�����������������ڻ��Ƶ���������ϵ |
| `MaxErrors` | 10 | ÿ�������ڼ�¼���쳣������������������� |
| `Timeout` | 5000ms | ��ʱ��ֵ��������ʱ��Ĳ����ᱻǿ�Ʋ��� |
| `MaxTagLength` | 1024 | ��ǩ��󳤶ȣ������ᱻ�ض� |
| `AttachParameter` | "traceparent" | HTTP/RPC ������ע�� TraceId �Ĳ���������ѭ W3C ��׼ |

��Щ����ͨ�����ǳ�������Ķ�̬�·������������ֶ����á�

#### ���ķ���

- **`NewSpan(String name)`**: ��õķ���������һ�������
- **`NewSpan(String name, Object? tag)`**: ������㲢�������ݱ�ǩ
- **`BuildSpan(String name)`**: ��ȡ�򴴽� SpanBuilder�����ڸ߼�����

### ISpan �ӿ�

���ܸ���Ƭ�Σ�����һ����������ʵ����

```csharp
public interface ISpan : IDisposable
{
    /// <summary>Ψһ��ʶ�����߳������ġ�Http��Rpc���ݣ���Ϊ�ڲ�Ƭ�εĸ���</summary>
    String Id { get; set; }

    /// <summary>����������ڱ�ʶ��ͬ���͵Ĳ���</summary>
    String Name { get; set; }

    /// <summary>����Ƭ�α�ʶ�����ڹ���������</summary>
    String? ParentId { get; set; }

    /// <summary>���ٱ�ʶ�������ڹ������Ƭ�Σ�����������ϵ�����߳������ġ�Http��Rpc����</summary>
    String TraceId { get; set; }

    /// <summary>��ʼʱ�䡣Unix����ʱ���</summary>
    Int64 StartTime { get; set; }

    /// <summary>����ʱ�䡣Unix����ʱ���</summary>
    Int64 EndTime { get; set; }

    /// <summary>�û���ֵ����¼�����ͱ�������ÿ�����ݿ�����������ǳ�ƽ̨����ͳ��</summary>
    Int64 Value { get; set; }

    /// <summary>���ݱ�ǩ����¼һЩ�������ݣ��������������Ӧ�����</summary>
    String? Tag { get; set; }

    /// <summary>������Ϣ����¼�쳣��Ϣ</summary>
    String? Error { get; set; }

    /// <summary>���ô�����Ϣ��ApiException����</summary>
    void SetError(Exception ex, Object? tag = null);

    /// <summary>�������ݱ�ǩ���ڲ�������󳤶Ƚض�</summary>
    void SetTag(Object tag);

    /// <summary>������㣬������ɼ�</summary>
    void Abandon();
}
```

#### �ؼ�����

| ���� | ���� | ˵�� |
|-----|------|------|
| `Id` | String | Span Ψһ��ʶ����ѭ W3C ��׼ |
| `ParentId` | String? | ���� Span ID�����ڹ��������� |
| `TraceId` | String | ��������ʶ��ͬһ�������е����� Span ������ͬ TraceId |
| `StartTime` | Int64 | ��ʼʱ�䣨Unix ���룩 |
| `EndTime` | Int64 | ����ʱ�䣨Unix ���룩 |
| `Value` | Int64 | �û���ֵ���ɼ�¼ҵ��ָ�꣨�����ݿ������� |
| `Tag` | String? | ���ݱ�ǩ����¼�����������Ӧ���ݵ� |
| `Error` | String? | ������Ϣ |

#### ���ķ���

- **`SetError(Exception ex, Object? tag)`**: ����쳣��`ApiException` ���ͻᱻ���⴦��
- **`SetTag(Object tag)`**: �������ݱ�ǩ��֧�ֶ��������Զ����л�
- **`Abandon()`**: ������ǰ��㣬�����ڹ�����Ч������404ɨ�裩

---

## ʹ�����ʵ��

### 1. ע�� ITracer

�� ASP.NET Core Ӧ���У�ͨ���ǳ���չע�룺

```csharp
using NewLife.Stardust.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ע�� DH.NStardust ���� ITracer ʵ��
builder.Services.AddStardust();
```

Stardust 历史平台地址已归档，当前请以对应仓库与现行部署信息为准

### 2. ��ȡ ITracer ʵ��

�ж��ַ�ʽ��ȡ `ITracer`��

```csharp
// ��ʽ1�����캯��ע�루�Ƽ���
public class MyService
{
    private readonly ITracer _tracer;
    
    public MyService(ITracer tracer)
    {
        _tracer = tracer;
    }
}

// ��ʽ2������ע��
public class MyService
{
    public ITracer? Tracer { get; set; }
}

// ��ʽ3��ʹ��ȫ�־�̬ʵ��
var tracer = DefaultTracer.Instance;
```

### 3. �������

#### �������

```csharp
using var span = _tracer?.NewSpan("MyOperation");
// ҵ���߼�
```

#### �����ݱ�ǩ�����

```csharp
var request = new { UserId = 123, Action = "Query" };
using var span = _tracer?.NewSpan("UserQuery", request);
// ҵ���߼�
```

#### ���쳣���������

```csharp
using var span = _tracer?.NewSpan("DatabaseQuery");
try
{
    // ִ�����ݿ��ѯ
    var result = await ExecuteQueryAsync();
}
catch (Exception ex)
{
    span?.SetError(ex, "��ѯʧ��");
    throw;
}
```

#### ��¼ҵ��ָ��

```csharp
using var span = _tracer?.NewSpan("BatchProcess");
var count = ProcessRecords();
span.Value = count;  // ��¼�����ļ�¼��
```

### 4. ��ʱ�������ʾ��

```csharp
public class DataRetentionService : IHostedService
{
    private readonly ITracer _tracer;
    private readonly StarServerSetting _setting;
    private TimerX? _timer;

    public DataRetentionService(StarServerSetting setting, ITracer tracer)
    {
        _setting = setting;
        _tracer = tracer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ÿ600��ִ��һ�Σ�����ӳ�����
        _timer = new TimerX(DoWork, null, 
            DateTime.Today.AddMinutes(Rand.Next(60)), 
            600 * 1000) 
        { 
            Async = true 
        };
        
        return Task.CompletedTask;
    }

    private void DoWork(Object? state)
    {
        var set = _setting;
        if (set.DataRetention <= 0) return;

        var time = DateTime.Now.AddDays(-set.DataRetention);
        var time2 = DateTime.Now.AddDays(-set.DataRetention2);
        var time3 = DateTime.Now.AddDays(-set.DataRetention3);

        // ������㣬��¼��������
        using var span = _tracer?.NewSpan("DataRetention", new { time, time2, time3 });
        try
        {
            // ɾ��������
            var rs = AppMinuteStat.DeleteBefore(time);
            XTrace.WriteLine("ɾ��[{0}]֮ǰ��AppMinuteStat����{1:n0}", time, rs);

            rs = TraceMinuteStat.DeleteBefore(time);
            XTrace.WriteLine("ɾ��[{0}]֮ǰ��TraceMinuteStat����{1:n0}", time, rs);
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.TryDispose();
        return Task.CompletedTask;
    }
}
```

### 5. ������Ч����

�� Web Ӧ���У�������������ɨ�����󣬿���ʹ�� `Abandon()` ������Щ��㣺

```csharp
public IActionResult ProcessRequest()
{
    using var span = _tracer?.NewSpan("WebRequest");
    
    // ��⵽��Ч������404ɨ�裩
    if (IsInvalidScan())
    {
        span?.Abandon();  // ������㣬������ͳ��
        return NotFound();
    }
    
    // ��������
    return Ok();
}
```

---

## �ֲ�ʽ��·׷��

### ����񴫵�

ISpan �ṩ����չ������֧��ͨ�� HTTP/RPC ���ݸ��������ģ�

```csharp
// HTTP ����ע��
var request = new HttpRequestMessage(HttpMethod.Get, url);
span?.Attach(request);  // ע�� traceparent ͷ
await httpClient.SendAsync(request);

// RPC ����ע��
var args = new { UserId = 123 };
span?.Attach(args);  // ע����ٲ���
await rpcClient.InvokeAsync("Method", args);
```

������յ�����󣬻��Զ����� `traceparent` �������ָ����������ġ�

### ����������

ͬһ�� `TraceId` �µ����� Span ���Զ��γɵ�������

```
TraceId: ac1b1e8617342790000015eb0ea2a6
���� DataRetention (��)
   ���� SQL:AppMinuteStat.DeleteBefore (��)
   ���� SQL:TraceMinuteStat.DeleteBefore (��)
   ���� SQL:TraceHourStat.DeleteBefore (��)
```

���ǳ����ƽ̨���Բ鿴��
- 示例追踪链接已归档，当前请以实际部署环境生成的链路地址为准

---

## ��������

### �鿴ͳ������

���ǳ����ƽ̨���Բ鿴���ͳ�ƣ�
- �����ܴ���
- �쳣����
- ƽ����ʱ������ʱ����С��ʱ
- QPS��ÿ����������

示例统计链接已归档，当前请以实际部署环境生成的统计地址为准

### ��������

ITracer �������ܲ������ԣ�
1. **��������**��ÿ����������ౣ�� `MaxSamples` ������������Ĭ��1����
2. **�쳣����**��ÿ����������ౣ�� `MaxErrors` ���쳣������Ĭ��10����
3. **��ʱ����**����ʱ���� `Timeout` �Ĳ���ǿ�Ʋ���
4. **ȫ��·����**������ `TraceFlag` �ĵ�����ȫ������

### ����ͳ�ƻ���

ÿ���������Ӧһ�� `SpanBuilder`�������ۼ�ͳ�ƣ�
- **Total**: �ܴ���
- **Errors**: �쳣����
- **Cost**: �ܺ�ʱ
- **MaxCost**: ����ʱ
- **MinCost**: ��С��ʱ

ÿ���������ڽ�����`SpanBuilder` ���ݴ���ϱ���Ȼ�����ü�������

---

## �߼�����

### ��������淶

����ʹ�÷ֲ����������ڷ���ͳ�ƣ�

```csharp
// �����
tracer.NewSpan("net:{Э��}:Receive");
tracer.NewSpan("net:tcp:Send");

// ���ݿ��
tracer.NewSpan("db:{����}:Select");
tracer.NewSpan("db:User:Insert");

// ҵ���
tracer.NewSpan("biz:{ģ��}:{����}");
tracer.NewSpan("biz:Order:Create");

// �ⲿ����
tracer.NewSpan("http:{����}:{����}");
tracer.NewSpan("http:PaymentApi:Pay");
```

### ��̬��������

���Զ�̬��������������

```csharp
var tracer = DefaultTracer.Instance;
tracer.Period = 30;           // ��Ϊ30������
tracer.MaxSamples = 5;        // ��������������
tracer.Timeout = 10000;       // ��ʱ��ֵ��Ϊ10��
tracer.MaxTagLength = 2048;   // ��ǩ���ȸ�Ϊ2K
```

ͨ����Щ�������ǳ��������ͳһ�·���

### �Զ��� ITracer ʵ��

����ʵ���Լ��� `ITracer`��

```csharp
public class CustomTracer : ITracer
{
    public Int32 Period { get; set; } = 15;
    public Int32 MaxSamples { get; set; } = 1;
    public Int32 MaxErrors { get; set; } = 10;
    // ... ʵ�ֽӿڷ���
    
    public ISpan NewSpan(String name)
    {
        // �Զ�����㴴���߼�
        var span = new CustomSpan { Name = name };
        span.Start();
        return span;
    }
}

// ע���Զ���ʵ��
DefaultTracer.Instance = new CustomTracer();
```

---

## �����Ż�

### �����

`DefaultTracer` �����˶���أ����� `ISpanBuilder` �� `ISpan` ʵ����

```csharp
public IPool<ISpanBuilder> BuilderPool { get; }
public IPool<ISpan> SpanPool { get; }
```

Ƶ����������������Զ��黹����أ����� GC ѹ����

### ��ǩ���ȿ���

���ڴ��Ͷ��󣬽�����Ʊ�ǩ���ݣ�

```csharp
// ���Ƽ�����¼���������
span.SetTag(largeObject);

// �Ƽ���ֻ��¼�ؼ���Ϣ
span.SetTag(new { Id = obj.Id, Name = obj.Name });
```

### �������

���ڷǹؼ�·�������������Դ�����㣺

```csharp
ISpan? span = null;
if (_tracer != null && IsImportantOperation())
{
    span = _tracer.NewSpan("ImportantOp");
}

try
{
    // ҵ���߼�
}
finally
{
    span?.Dispose();
}
```

---

## �쳣����

### ApiException ���⴦��

ҵ���쳣 `ApiException` ���ᱻ���Ϊ����

```csharp
catch (ApiException aex)
{
    span?.SetError(aex, request);
    // ��¼Ϊ������㣬Tag�а���ҵ�������
}
catch (Exception ex)
{
    span?.SetError(ex, request);
    // ��¼Ϊ�쳣��㣬Error�а����쳣��Ϣ
}
```

### �쳣���

ÿ���쳣���Զ������������쳣��㣬���ڰ��쳣����ͳ�ƣ�

```csharp
// ԭʼ���
using var span = tracer.NewSpan("DatabaseQuery");

try
{
    // �׳��쳣
    throw new TimeoutException("��ѯ��ʱ");
}
catch (Exception ex)
{
    span.SetError(ex, query);
    // �Զ����� "ex:TimeoutException" ���
}
```

---

## ��������

### 1. ����������ǳ�ƽ̨��������

������¼��㣺
- ȷ����ע���ǳ���չ��`services.AddStardust()`
- ����������ӵ��ǳ�������
- ���Ӧ�����ǳ�ƽ̨�Ƿ���ע��
- �鿴������־��ȷ�������������

### 2. ��������̫�٣�

��������������
```csharp
tracer.MaxSamples = 10;    // ��������������
tracer.MaxErrors = 50;     // �����쳣������
tracer.Timeout = 1000;     // ���ͳ�ʱ��ֵ
```

### 3. ��ιر���㣿

```csharp
// ��ʽ1����ע�� ITracer
// services.AddStardust();  // ע�͵�

// ��ʽ2��ʹ�ÿ�ʵ��
DefaultTracer.Instance = null;
```

### 4. ��β鿴����������ݣ�

DefaultTracer ���������־��

```
Tracer[DataRetention] Total=10 Errors=0 Speed=0.02tps Cost=1500ms MaxCost=2000ms MinCost=1000ms
```

### 5. ���������� Span ������

���ᡣ`ISpan` ʹ�� `AsyncLocal<ISpan>` ���������ģ�ÿ���첽���̶�����

```csharp
public static AsyncLocal<ISpan?> Current { get; }
```

---

## �ο�����

- Stardust 历史平台地址已归档
- **W3C Trace Context**: https://www.w3.org/TR/trace-context/
- **Դ��ֿ�**: https://github.com/PeiKeSmart/DH.NCore
- 历史文档已归档，当前请以仓库内 Doc 为准

---

## ������־

- **2024-12-16**: �����ĵ����������ʵ����ʾ������
- **2024-08**: ֧�� .NET 9.0
- **2023-11**: ���� `Abandon()` ����
- **2023-06**: �Ż�����أ���������
- **2022**: ��ʼ�汾����
