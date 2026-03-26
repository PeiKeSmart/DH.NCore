# JSON ���л�

## ����

DH.NCore �ṩ���������� JSON ���л��ͷ����л����ܣ�ͨ�� `JsonHelper` ��չ�������Է���ؽ��ж����� JSON �ַ�����ת�������� `FastJson` ʵ�֣�ͬʱ֧���л��� `System.Text.Json`��

**�����ռ�**��`NewLife.Serialization`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **������**������ `FastJson` ʵ�֣����ⲿ����
- **������**����Գ��������Ż���֧�ֶ����
- **��չ����**��`ToJson()` �� `ToJsonEntity<T>()` �������
- **�������**��֧���շ����������Կ�ֵ��������ʽ����
- **����ת��**���Զ�������������ת��
- **���л�**��֧���л��� `System.Text.Json` ʵ��

## ���ٿ�ʼ

### ���л�

```csharp
using NewLife.Serialization;

// �򵥶������л�
var user = new { Id = 1, Name = "����", Age = 25 };
var json = user.ToJson();
// {"Id":1,"Name":"����","Age":25}

// ��ʽ�����
var jsonIndented = user.ToJson(true);
// {
//   "Id": 1,
//   "Name": "����",
//   "Age": 25
// }

// �շ�����
var jsonCamel = user.ToJson(false, true, true);
// {"id":1,"name":"����","age":25}
```

### �����л�

```csharp
using NewLife.Serialization;

var json = """{"Id":1,"Name":"����","Age":25}""";

// �����л�Ϊָ������
var user = json.ToJsonEntity<User>();

// �����л�Ϊ��̬�ֵ�
var dict = json.DecodeJson();
var name = dict["Name"];  // "����"
```

## API �ο�

### ToJson - ���л�

```csharp
// �������л�
public static String ToJson(this Object value, Boolean indented = false)

// ��������
public static String ToJson(this Object value, Boolean indented, Boolean nullValue, Boolean camelCase)

// ʹ�����ö���
public static String ToJson(this Object value, JsonOptions jsonOptions)
```

**����˵��**��
- `indented`���Ƿ�������ʽ����Ĭ�� false
- `nullValue`���Ƿ������ֵ��Ĭ�� true
- `camelCase`���Ƿ�ʹ���շ�������Ĭ�� false

**ʾ��**��
```csharp
var obj = new
{
    Id = 1,
    Name = "����",
    Description = (String?)null,
    CreateTime = DateTime.Now
};

// Ĭ�����
obj.ToJson();
// {"Id":1,"Name":"����","Description":null,"CreateTime":"2025-01-07 12:00:00"}

// ���Կ�ֵ
obj.ToJson(false, false, false);
// {"Id":1,"Name":"����","CreateTime":"2025-01-07 12:00:00"}

// �շ����� + ��ʽ��
obj.ToJson(true, true, true);
```

### ToJsonEntity - �����л�

```csharp
// ���ͷ���
public static T? ToJsonEntity<T>(this String json)

// ָ������
public static Object? ToJsonEntity(this String json, Type type)
```

**ʾ��**��
```csharp
var json = """{"id":1,"name":"����","roles":["admin","user"]}""";

// �����л�Ϊ��
public class User
{
    public Int32 Id { get; set; }
    public String Name { get; set; }
    public String[] Roles { get; set; }
}

var user = json.ToJsonEntity<User>();
Console.WriteLine(user.Name);  // ����
Console.WriteLine(user.Roles[0]);  // admin
```

### DecodeJson - ����Ϊ�ֵ�

```csharp
public static IDictionary<String, Object?>? DecodeJson(this String json)
```

�� JSON �ַ�������Ϊ�ֵ䣬�����ڶ�̬���ʳ�����

**ʾ��**��
```csharp
var json = """{"code":0,"data":{"id":1,"name":"test"},"message":"ok"}""";

var dict = json.DecodeJson();
var code = dict["code"].ToInt();  // 0
var data = dict["data"] as IDictionary<String, Object>;
var id = data["id"].ToInt();  // 1
```

### JsonOptions - ����ѡ��

```csharp
public class JsonOptions
{
    /// <summary>ʹ���շ�������Ĭ��false</summary>
    public Boolean CamelCase { get; set; }
    
    /// <summary>���Կ�ֵ��Ĭ��false</summary>
    public Boolean IgnoreNullValues { get; set; }
    
    /// <summary>����ѭ�����á�Ĭ��false</summary>
    public Boolean IgnoreCycles { get; set; }
    
    /// <summary>������ʽ����Ĭ��false</summary>
    public Boolean WriteIndented { get; set; }
    
    /// <summary>ʹ������ʱ���ʽ��Ĭ��false</summary>
    public Boolean FullTime { get; set; }
    
    /// <summary>ö��ʹ���ַ�����Ĭ��falseʹ������</summary>
    public Boolean EnumString { get; set; }
    
    /// <summary>��������Ϊ�ַ���������JS���ȶ�ʧ��Ĭ��false</summary>
    public Boolean Int64AsString { get; set; }
}
```

**ʾ��**��
```csharp
var options = new JsonOptions
{
    CamelCase = true,
    IgnoreNullValues = true,
    WriteIndented = true,
    EnumString = true
};

var json = obj.ToJson(options);
```

### Format - ��ʽ�� JSON

```csharp
public static String Format(String json)
```

��ѹ���� JSON �ַ�����ʽ��Ϊ�׶���ʽ��

**ʾ��**��
```csharp
var json = """{"id":1,"name":"test","items":[1,2,3]}""";
var formatted = JsonHelper.Format(json);
// {
//   "id":  1,
//   "name":  "test",
//   "items":  [
//     1,
//     2,
//     3
//   ]
// }
```

## IJsonHost �ӿ�

`IJsonHost` �� JSON ���л��ĺ��Ľӿڣ������л���ͬ��ʵ�֣�

```csharp
public interface IJsonHost
{
    IServiceProvider ServiceProvider { get; set; }
    JsonOptions Options { get; set; }
    
    String Write(Object value, Boolean indented = false, Boolean nullValue = true, Boolean camelCase = false);
    String Write(Object value, JsonOptions jsonOptions);
    Object? Read(String json, Type type);
    Object? Convert(Object obj, Type targetType);
    Object? Parse(String json);
    IDictionary<String, Object?>? Decode(String json);
}
```

### �л�ʵ��

```csharp
// Ĭ��ʹ�� FastJson
JsonHelper.Default = new FastJson();

// �л��� System.Text.Json��.NET 5+��
JsonHelper.Default = new SystemJson();
```

## ʹ�ó���

### 1. Web API ���ݽ���

```csharp
// ���л���Ӧ
public class ApiResult<T>
{
    public Int32 Code { get; set; }
    public String? Message { get; set; }
    public T? Data { get; set; }
}

var result = new ApiResult<User>
{
    Code = 0,
    Message = "success",
    Data = new User { Id = 1, Name = "test" }
};

// ʹ���շ�������ǰ���Ѻã�
var json = result.ToJson(false, true, true);

// ������Ӧ
var response = json.ToJsonEntity<ApiResult<User>>();
```

### 2. �����ļ�����

```csharp
// ��ȡ JSON ����
var json = File.ReadAllText("config.json");
var config = json.ToJsonEntity<AppConfig>();

// ��������
var newJson = config.ToJson(true);  // ��ʽ�������Ķ�
File.WriteAllText("config.json", newJson);
```

### 3. ��־��¼

```csharp
public void LogRequest(Object request)
{
    // ���л��������������־
    var json = request.ToJson();
    XTrace.WriteLine($"Request: {json}");
}
```

### 4. ��̬���ݴ���

```csharp
// ������ȷ���ṹ�� JSON
var json = await httpClient.GetStringAsync(url);
var dict = json.DecodeJson();

if (dict.TryGetValue("error", out var error))
{
    throw new Exception(error?.ToString());
}

var data = dict["data"] as IDictionary<String, Object>;
// ��̬�����ֶ�...
```

### 5. ���������;�������

```csharp
// JavaScript �޷���ȷ��ʾ���� 2^53 ������
var options = new JsonOptions { Int64AsString = true };

var obj = new { Id = 9007199254740993L };
var json = obj.ToJson(options);
// {"Id":"9007199254740993"}  // �ַ�����ʽ�����⾫�ȶ�ʧ
```

## �������ʹ���

### ����ʱ��

```csharp
var obj = new { Time = DateTime.Now };

// Ĭ�ϸ�ʽ
obj.ToJson();
// {"Time":"2025-01-07 12:00:00"}

// ���� ISO ��ʽ
var options = new JsonOptions { FullTime = true };
obj.ToJson(options);
// {"Time":"2025-01-07T12:00:00.0000000+08:00"}
```

### ö��

```csharp
public enum Status { Pending, Active, Closed }

var obj = new { Status = Status.Active };

// Ĭ��ʹ������
obj.ToJson();
// {"Status":1}

// ʹ���ַ���
var options = new JsonOptions { EnumString = true };
obj.ToJson(options);
// {"Status":"Active"}
```

### �ֽ�����

```csharp
var obj = new { Data = new Byte[] { 1, 2, 3, 4 } };

// Ĭ�� Base64
obj.ToJson();
// {"Data":"AQIDBA=="}
```

## ���ʵ��

### 1. �������ö���

```csharp
// ����ȫ������
public static class JsonConfig
{
    public static readonly JsonOptions Api = new()
    {
        CamelCase = true,
        IgnoreNullValues = true
    };
    
    public static readonly JsonOptions Log = new()
    {
        WriteIndented = false,
        IgnoreNullValues = true
    };
}

// ʹ��
var json = data.ToJson(JsonConfig.Api);
```

### 2. ������ֵ

```csharp
// �����л�ʱע���ֵ
var user = json.ToJsonEntity<User>();
if (user == null)
{
    // JSON Ϊ null �����ʧ��
}

// ��ʹ�ÿ��ַ������
if (json.IsNullOrEmpty()) return;
var user = json.ToJsonEntity<User>();
```

### 3. ������������

```csharp
// ���ڴ������ݣ����Ƿ�������
// ����һ�������л�/�����л��������
```

## ����˵��

- `FastJson` ��Գ��������Ż����ʺϴ����Ӧ��
- ���ڸ�����Ҫ�󣬿��л��� `System.Text.Json`
- ����Ƶ������ `JsonOptions`�����鸴��
- �ַ�������ʹ�ö�����Ż�

## �������

- [���������л� Binary](binary-���������л�Binary.md)
- [XML ���л�](xml-XML���л�Xml.md)
- [����ϵͳ Config](config-����ϵͳConfig.md)
