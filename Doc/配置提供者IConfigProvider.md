# IConfigProvider ������ϵʹ��˵��

���ĵ����� NewLife.Configuration ������ϵ�ļܹ����÷�����չ�㣬������ DH.NCore �ֿ⡣

## 1. �ܹ�����

- ���ĳ���
  - `IConfigProvider`�������ṩ��ͳһ�ӿڣ���¶��ֵ���ʡ����ζη��ʡ�ģ�� Load/Save/Bind�����֪ͨ��
  - `IConfigSection`�����öΣ��ڵ㣩����/ֵ/ע�����Ӽ������Σ��ṹ��
  - `GetConfigCallback`����ȡ���õ�ί�У�������ע�뵽����ģ���н��а�����ȡ��
- �������
  - `ConfigProvider`��ʵ�� `IConfigProvider` �Ļ��࣬�ṩ�����ء���·�����ʡ�ģ��ӳ�䡢������֪ͨ��Ĭ���ṩ��ע���빤��������
  - `FileConfigProvider`���ļ����ṩ�߻��࣬��װ�ļ���д����ѯ�ȼ��أ�`TimerX`����
- ����ʵ��
  - `XmlConfigProvider`��XML �ļ���
  - `InIConfigProvider`��INI �ļ���
  - `JsonConfigProvider`��JSON �ļ���֧��ע�͵�Ԥ��������
  - `HttpConfigProvider`���������ģ��ǳ��ȣ��������ػ��桢�汾�������ϱ���֧�ֶ�ʱˢ�¡�
  - `ApolloConfigProvider`����� Apollo �����䣨�����ռ�ۺ϶�ȡ����
  - `CompositeConfigProvider`�������ṩ�ߣ��ۺ϶���ṩ�ߣ���ȡ���ȡ�����������ԡ�
- ������ģ��
  - `Config<T>`������ģ�ͻ��࣬`Current` ͨ����ע `ConfigAttribute` �Զ�ѡ���ṩ�߲�����/�󶨡�
  - `ConfigHelper`����ģ����������֮�����ӳ�䣨`MapTo/MapFrom`����֧�����顢`IList<T>`�����Ӷ����ֵ䡣
  - `IConfigMapping`���Զ���ӳ��ӿڣ��û�����ģ����ʵ���Ի����ȫ���Ƶ�ӳ���߼���

## 2. �����÷�

- ����ģ��

```csharp
[Config("core", provider: null)] // ʹ��Ĭ���ṩ�ߣ���ȫ���л���
public class CoreConfig : Config<CoreConfig>
{
    [Description("ȫ�ֵ��ԡ�XTrace.Debug")] public bool Debug { get; set; }
    public string? LogPath { get; set; }
    public SysConfig Sys { get; set; } = new();
}

public class SysConfig
{
    [Description("���ڱ�ʶϵͳ��Ӣ�����������пո�")] public string Name { get; set; } = "";
    public string? DisplayName { get; set; }
}
```

- ����/����

```csharp
var cfg = CoreConfig.Current;      // �״λ�󶨲��Զ����̣����½���
cfg.Debug = true;
cfg.Save();
```

- ֱ��ʹ���ṩ��

```csharp
var prv = ConfigProvider.Create("config")!; // xml Ĭ�� .config
prv.Init("core");
prv.LoadAll();
var debug = prv["Debug"];           // ��·��֧��ð�ŷָ���"Sys:Name"
var section = prv.GetSection("Sys:Name");
```

- ���ȸ���

```csharp
var cfg = new CoreConfig();
var prv = new JsonConfigProvider { FileName = "Config/core.json" };
prv.Bind(cfg, autoReload: true); // �ļ����/Զ�����ͽ�ˢ�� cfg
```

- �����ṩ��

```csharp
var local = new JsonConfigProvider { FileName = "Config/appsettings.json" };
var remote = new HttpConfigProvider { Server = "http://conf", AppId = "Demo" };
var composite = new CompositeConfigProvider(local, remote);
var name = composite["Sys:Name"]; // ��ȡʱ��˳�����
```

## 3. ��չ��

- �½��ļ��ṩ�ߣ��̳� `FileConfigProvider`����д `OnRead` �� `GetString/OnWrite` ���ɡ�
- �½�Զ���ṩ�ߣ��̳� `ConfigProvider`��ʵ�� `LoadAll/SaveAll` �붨ʱˢ�£���ѡ����
- ע��ΪĬ�ϣ�`ConfigProvider.Register<MyProvider>("my");`�����ͨ�� `ConfigProvider.Create("my")` ʹ�á�

## 4. ��Ϊ��Լ��

- ��·���﷨��`A:B:C` ��ʾ�������ꡣ
- `Keys` Ĭ��ֻ���ظ��µ�һ���������ʵ�ֿɸ��Ƿ����������ϡ�
- `IsNew` ����ָʾ����Դ�Ƿ��״δ�����`Config<T>.Current` ��ݴ˾����Ƿ�־û�Ĭ��ֵ��
- ע�ͣ�ģ�������ϵ� `DescriptionAttribute/DisplayNameAttribute` ��д��������ע�͡�
- �б������飺`ConfigHelper` ֧�� `T[]` �� `IList<T>`����Ԫ����Ԫ�ػ��������ת����

## 5. ���ֿ��Ż��㣨�Ѵ�����

- �޸���`ConfigHelper.MapToList` �Ի�Ԫ����Ԫ�ؽ��� `ChangeType` ת�������� `List<int>` ��д���ַ����б���
- ��׳�ԣ�`ConfigProvider.Keys` ����ǰ���� `EnsureLoad()`������δ���ؼ����ʵ��µĿռ������С�
- ������ȫ��`ConfigProvider` �İ󶨼��ϸ�Ϊ `ConcurrentDictionary`����ֹ���֪ͨ�ڼ�ö���쳣��

## 6. ע������

- ���ܣ�����������ӳ�䲻Ӧ�����ȵ��Ƶ·��������Ƶ�����ʣ��뻺�����û�ʹ��ǿ���Ͱ󶨡�
- �̰߳�ȫ��ģ�Ͱ���ˢ�»ص���Ҫִ�к�ʱ��������������֪ͨ��·��
- �����ԣ���� .NET Framework �� .NET Standard �������䣬����ʹ��ƽ̨ר�� API��

---
���ϡ�
