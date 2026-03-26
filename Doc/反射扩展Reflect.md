# ������չ Reflect

## ����

`Reflect` �� DH.NCore �еĸ����ܷ��乤���࣬�ṩ���ͻ�ȡ���������á����Զ�д�����󿽱��ȹ��ܡ�֧��˽�г�Ա���ʡ����Դ�Сдƥ�䣬��ͨ�� `IReflect` �ӿ�֧�ֿ��滻�ķ���ʵ�֡�

**�����ռ�**��`NewLife.Reflection`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **������**��Ĭ��ʵ�ֻ��ڻ��棬֧���л�Ϊ Emit ������ʵ��
- **������**�����з���������չ������ʽ�ṩ
- **������**��֧��˽�г�Ա����̬��Ա���̳г�Ա�ķ���
- **�����**��֧�ֺ��Դ�Сд�ĳ�Աƥ��
- **����չ**��ͨ�� `IReflect` �ӿ�֧���Զ���ʵ��

## ���ٿ�ʼ

```csharp
using NewLife.Reflection;

// ����ʵ��
var obj = typeof(MyClass).CreateInstance();

// ���÷���
obj.Invoke("DoWork", "param1", 123);

// ��ȡ����
var value = obj.GetValue("Name");

// ��������
obj.SetValue("Name", "NewValue");

// ���󿽱�
var target = new MyClass();
target.Copy(source);
```

## API �ο�

### ���ͻ�ȡ

#### GetTypeEx

```csharp
public static Type? GetTypeEx(this String typeName)
```

�����������ƻ�ȡ���ͣ���������ǰĿ¼ DLL ���Զ����ء�

**ʾ��**��
```csharp
// ��ȡϵͳ����
var type1 = "System.String".GetTypeEx();

// ��ȡ�������ռ������
var type2 = "MyApp.Models.User".GetTypeEx();

// ��ȡ�����޶�������
var type3 = "MyApp.Models.User, MyApp".GetTypeEx();
```

### ��Ա��ȡ

#### GetMethodEx

```csharp
public static MethodInfo? GetMethodEx(this Type type, String name, params Type[] paramTypes)
```

��ȡ������֧�ֲ�������ƥ�䡣

**ʾ��**��
```csharp
// ��ȡ�޲η���
var method1 = typeof(MyClass).GetMethodEx("DoWork");

// ��ȡ���η���
var method2 = typeof(MyClass).GetMethodEx("DoWork", typeof(String), typeof(Int32));
```

#### GetMethodsEx

```csharp
public static MethodInfo[] GetMethodsEx(this Type type, String name, Int32 paramCount = -1)
```

��ȡָ�����Ƶķ������ϣ�֧�ְ������������ˡ�

**ʾ��**��
```csharp
// ��ȡ������Ϊ DoWork �ķ���
var methods1 = typeof(MyClass).GetMethodsEx("DoWork");

// ��ȡ��������Ϊ 2 �� DoWork ����
var methods2 = typeof(MyClass).GetMethodsEx("DoWork", 2);
```

#### GetPropertyEx

```csharp
public static PropertyInfo? GetPropertyEx(this Type type, String name, Boolean ignoreCase = false)
```

��ȡ���ԣ�����˽�С���̬�������Ա��

**ʾ��**��
```csharp
// ��ȷƥ��
var prop1 = typeof(MyClass).GetPropertyEx("Name");

// ���Դ�Сд
var prop2 = typeof(MyClass).GetPropertyEx("name", true);

// ��ȡ˽������
var prop3 = typeof(MyClass).GetPropertyEx("_internalValue");
```

#### GetFieldEx

```csharp
public static FieldInfo? GetFieldEx(this Type type, String name, Boolean ignoreCase = false)
```

��ȡ�ֶΣ�����˽�С���̬�������Ա��

**ʾ��**��
```csharp
var field = typeof(MyClass).GetFieldEx("_count");
```

#### GetMemberEx

```csharp
public static MemberInfo? GetMemberEx(this Type type, String name, Boolean ignoreCase = false)
```

��ȡ��Ա�����Ի��ֶΣ������ȷ������ԡ�

**ʾ��**��
```csharp
var member = typeof(MyClass).GetMemberEx("Name", true);
```

#### GetFields / GetProperties

```csharp
public static IList<FieldInfo> GetFields(this Type type, Boolean baseFirst)
public static IList<PropertyInfo> GetProperties(this Type type, Boolean baseFirst)
```

��ȡ�������л����ֶ�/�����б���

**����˵��**��
- `baseFirst`���Ƿ�����Ա��������

**ʾ��**��
```csharp
// ��ȡ���п����л����ԣ���������
var props = typeof(MyClass).GetProperties(baseFirst: true);

// ��ȡ���п����л��ֶ�
var fields = typeof(MyClass).GetFields(baseFirst: false);
```

### ʵ�������뷽������

#### CreateInstance

```csharp
public static Object? CreateInstance(this Type type, params Object?[] parameters)
```

���䴴��ָ�����͵�ʵ����

**ʾ��**��
```csharp
// �����޲ι��캯��
var obj1 = typeof(MyClass).CreateInstance();

// ���ô��ι��캯��
var obj2 = typeof(MyClass).CreateInstance("name", 123);
```

#### Invoke

```csharp
public static Object? Invoke(this Object target, String name, params Object?[] parameters)
public static Object? Invoke(this Object? target, MethodBase method, params Object?[]? parameters)
```

������÷�����

**ʾ��**��
```csharp
var obj = new MyClass();

// ����ʵ������
var result = obj.Invoke("Calculate", 10, 20);

// ���þ�̬������target Ϊ���ͣ�
var result2 = typeof(MyClass).Invoke("StaticMethod", "param");

// ����˽�з���
var result3 = obj.Invoke("PrivateMethod");
```

#### TryInvoke

```csharp
public static Boolean TryInvoke(this Object target, String name, out Object? value, params Object?[] parameters)
```

���Ե��÷�����������ʱ���� false �����׳��쳣��

**ʾ��**��
```csharp
if (obj.TryInvoke("MaybeExists", out var result, "param"))
{
    Console.WriteLine($"���: {result}");
}
else
{
    Console.WriteLine("����������");
}
```

#### InvokeWithParams

```csharp
public static Object? InvokeWithParams(this Object? target, MethodBase method, IDictionary? parameters)
```

ʹ���ֵ�������÷������ʺϲ�����ƥ�䳡����

**ʾ��**��
```csharp
var parameters = new Dictionary<String, Object>
{
    ["name"] = "test",
    ["count"] = 10
};
var result = obj.InvokeWithParams(method, parameters);
```

### ���Զ�д

#### GetValue

```csharp
public static Object? GetValue(this Object target, String name, Boolean throwOnError = true)
public static Object? GetValue(this Object? target, MemberInfo member)
```

��ȡ����/�ֶ�ֵ��

**ʾ��**��
```csharp
var obj = new MyClass { Name = "test" };

// �����ƻ�ȡ
var name = obj.GetValue("Name");

// ������ʱ���� null �������쳣
var value = obj.GetValue("NotExists", throwOnError: false);

// ����Ա��ȡ
var prop = typeof(MyClass).GetPropertyEx("Name");
var name2 = obj.GetValue(prop);
```

#### SetValue

```csharp
public static Boolean SetValue(this Object target, String name, Object? value)
public static void SetValue(this Object target, MemberInfo member, Object? value)
```

��������/�ֶ�ֵ��

**ʾ��**��
```csharp
var obj = new MyClass();

// ����������
obj.SetValue("Name", "newValue");

// ����Ա����
var prop = typeof(MyClass).GetPropertyEx("Name");
obj.SetValue(prop, "anotherValue");

// ����Ƿ����óɹ�
if (obj.SetValue("MaybeExists", "value"))
{
    Console.WriteLine("���óɹ�");
}
```

### ���󿽱�

#### Copy

```csharp
public static void Copy(this Object target, Object src, Boolean deep = false, params String[] excludes)
public static void Copy(this Object target, IDictionary<String, Object?> dic, Boolean deep = false)
```

��Դ������ֵ俽�����ݵ�Ŀ�����

**����˵��**��
- `deep`���Ƿ���ȿ���������ֵ�������ã�
- `excludes`��Ҫ�ų��ĳ�Ա����

**ʾ��**��
```csharp
var source = new User { Name = "����", Age = 25 };
var target = new UserDto();

// ǳ����
target.Copy(source);

// ���
target.Copy(source, deep: true);

// �ų�ĳЩ�ֶ�
target.Copy(source, excludes: "Password", "Secret");

// ���ֵ俽��
var dic = new Dictionary<String, Object?>
{
    ["Name"] = "����",
    ["Age"] = 30
};
target.Copy(dic);
```

### ���͸���

#### GetElementTypeEx

```csharp
public static Type? GetElementTypeEx(this Type type)
```

��ȡ���͵�Ԫ�����ͣ����ϡ�����ȣ���

**ʾ��**��
```csharp
typeof(List<String>).GetElementTypeEx()   // typeof(String)
typeof(String[]).GetElementTypeEx()       // typeof(String)
typeof(Dictionary<String, Int32>).GetElementTypeEx()  // typeof(KeyValuePair<String, Int32>)
```

#### ChangeType

```csharp
public static Object? ChangeType(this Object? value, Type conversionType)
public static TResult? ChangeType<TResult>(this Object? value)
```

����ת����

**ʾ��**��
```csharp
// ����ת��
var num = "123".ChangeType<Int32>();     // 123
var date = "2024-01-15".ChangeType<DateTime>();

// �Ƿ���ת��
var value = "true".ChangeType(typeof(Boolean));
```

#### GetName

```csharp
public static String GetName(this Type type, Boolean isfull = false)
```

��ȡ���͵��Ѻ����ơ�

**ʾ��**��
```csharp
typeof(List<String>).GetName()        // "List<String>"
typeof(List<String>).GetName(true)    // "System.Collections.Generic.List<System.String>"
typeof(Dictionary<String, Int32>).GetName()  // "Dictionary<String, Int32>"
```

## ʹ�ó���

### 1. ORM ʵ��ӳ��

```csharp
public class EntityMapper
{
    public T Map<T>(IDataReader reader) where T : new()
    {
        var entity = new T();
        var props = typeof(T).GetProperties(baseFirst: false);
        
        foreach (var prop in props)
        {
            var ordinal = reader.GetOrdinal(prop.Name);
            if (ordinal >= 0 && !reader.IsDBNull(ordinal))
            {
                var value = reader.GetValue(ordinal);
                entity.SetValue(prop, value);
            }
        }
        
        return entity;
    }
}
```

### 2. ���ð�

```csharp
public class ConfigBinder
{
    public void Bind(Object target, IConfiguration config)
    {
        var props = target.GetType().GetProperties(baseFirst: true);
        
        foreach (var prop in props)
        {
            var value = config[prop.Name];
            if (value != null)
            {
                var converted = value.ChangeType(prop.PropertyType);
                target.SetValue(prop, converted);
            }
        }
    }
}
```

### 3. ���ϵͳ

```csharp
public class PluginLoader
{
    public IPlugin? LoadPlugin(String typeName)
    {
        var type = typeName.GetTypeEx();
        if (type == null) return null;
        
        return type.CreateInstance() as IPlugin;
    }
    
    public void InvokeAction(IPlugin plugin, String action, params Object[] args)
    {
        if (plugin.TryInvoke(action, out var result, args))
        {
            Console.WriteLine($"ִ�гɹ�: {result}");
        }
    }
}
```

### 4. DTO ת��

```csharp
public static class DtoExtensions
{
    public static TDto ToDto<TDto>(this Object entity) where TDto : new()
    {
        var dto = new TDto();
        dto.Copy(entity);
        return dto;
    }
    
    public static void UpdateFrom(this Object entity, Object dto, params String[] excludes)
    {
        entity.Copy(dto, excludes: excludes);
    }
}

// ʹ��
var dto = user.ToDto<UserDto>();
user.UpdateFrom(dto, "Id", "CreateTime");
```

## ���ʵ��

### 1. ʹ�� TryInvoke �����쳣

```csharp
// ? �Ƽ���ʹ�� TryInvoke
if (obj.TryInvoke("Method", out var result))
{
    // �������
}

// ? ���Ƽ����������쳣
try
{
    var result = obj.Invoke("Method");
}
catch (XException) { }
```

### 2. ���淴��Ԫ����

```csharp
// ? �Ƽ������� PropertyInfo
private static readonly PropertyInfo _nameProp = typeof(User).GetPropertyEx("Name");

public String GetName(User user) => user.GetValue(_nameProp) as String;

// ? ���Ƽ���ÿ�ζ�����
public String GetName(User user) => user.GetValue("Name") as String;
```

### 3. ʹ�ú��Դ�Сдƥ��

```csharp
// ���� JSON �����л��ȳ���
var value = obj.GetValue("username", throwOnError: false);
if (value == null)
{
    // ���Ժ��Դ�Сд
    var member = obj.GetType().GetMemberEx("username", ignoreCase: true);
    if (member != null) value = obj.GetValue(member);
}
```

## ����˵��

- Ĭ�� `DefaultReflect` ʵ��ʹ�û��棬�ʺϴ��������
- ��Ƶ���䳡�����л�Ϊ `EmitReflect` ʵ�֣�
  ```csharp
  Reflect.Provider = new EmitReflect();
  ```
- `GetProperties` �� `GetFields` ����ᱻ����
- ��Ա����ʹ���ֵ仺�棬�״η��ʺ����ܽӽ�ֱ�ӵ���

## �������

- [����ʱ��Ϣ Runtime](runtime-����ʱ��ϢRuntime.md)
- [�ű����� ScriptEngine](script_engine-�ű�����ScriptEngine.md)
- [�������� ObjectContainer](object_container-��������ObjectContainer.md)
