# ����ת�� Utility

## ����

`Utility` �� DH.NCore ��������Ĺ����࣬�ṩ��Ч����ȫ������ת����չ����������ת��������֧��Ĭ��ֵ����ת��ʧ��ʱ����Ĭ��ֵ�����׳��쳣����������ճ������е�����ת��������

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ȫת��**������ת��ʧ��ʱ����Ĭ��ֵ�����׳��쳣
- **��չ����**��ֱ���ڶ����ϵ��� `.ToInt()`��`.ToDateTime()` ��
- **������֧��**��֧���ַ�����ȫ���ַ����ֽ����顢ʱ����ȶ�������
- **������**����Գ�����������Ż������ⲻ��Ҫ���ڴ����
- **����չ**��ͨ�� `DefaultConvert` ��֧���Զ���ת���߼�

## ���ٿ�ʼ

```csharp
using NewLife;

// �ַ���ת����
var num = "123".ToInt();           // 123
var num2 = "abc".ToInt(-1);        // -1��ת��ʧ�ܷ���Ĭ��ֵ��

// �ַ���תʱ��
var dt = "2024-01-15".ToDateTime();
var dt2 = "invalid".ToDateTime();  // DateTime.MinValue

// ����ת����
var flag = "true".ToBoolean();     // true
var flag2 = "1".ToBoolean();       // true
var flag3 = "yes".ToBoolean();     // true
```

## API �ο�

### ����ת��

#### ToInt

```csharp
public static Int32 ToInt(this Object? value, Int32 defaultValue = 0)
```

������ת��Ϊ32λ������

**֧�ֵ���������**��
- �ַ�������ȫ�����֣�
- �ֽ����飨С����1-4�ֽڣ�
- DateTime��תΪUnix�룬����ʱ��ת����
- DateTimeOffset��תΪUnix�룩
- ʵ�� `IConvertible` ������

**ʾ��**��
```csharp
// ����ת��
"123".ToInt()                    // 123
"  456  ".ToInt()                // 456���Զ�ȥ���ո�
"������".ToInt()                 // 123��֧��ȫ�����֣�
"1,234,567".ToInt()              // 1234567��֧��ǧ��λ��

// �ֽ�����ת����С����
new Byte[] { 0x01 }.ToInt()                    // 1
new Byte[] { 0x01, 0x00 }.ToInt()              // 1
new Byte[] { 0x01, 0x00, 0x00, 0x00 }.ToInt()  // 1

// ʱ��תUnix��
DateTime.Now.ToInt()             // ��ǰUnixʱ������룩

// ת��ʧ�ܷ���Ĭ��ֵ
"abc".ToInt()                    // 0
"abc".ToInt(-1)                  // -1
((Object?)null).ToInt()          // 0
```

#### ToLong

```csharp
public static Int64 ToLong(this Object? value, Int64 defaultValue = 0)
```

������ת��Ϊ64λ��������

**���⴦��**��
- DateTime תΪ Unix ���루����ʱ��ת����
- �ֽ�����֧�� 1-8 �ֽ�

**ʾ��**��
```csharp
"9223372036854775807".ToLong()   // Int64.MaxValue
DateTime.Now.ToLong()            // ��ǰUnixʱ��������룩
```

### ������ת��

#### ToDouble

```csharp
public static Double ToDouble(this Object? value, Double defaultValue = 0)
```

������ת��Ϊ˫���ȸ�������

**ʾ��**��
```csharp
"3.14".ToDouble()                // 3.14
"3.14E+10".ToDouble()            // 31400000000��֧�ֿ�ѧ��������
"1,234.56".ToDouble()            // 1234.56��֧��ǧ��λ��
```

#### ToDecimal

```csharp
public static Decimal ToDecimal(this Object? value, Decimal defaultValue = 0)
```

������ת��Ϊ�߾��ȸ������������ڽ��ڼ���Ⱦ���Ҫ��ߵĳ�����

**ʾ��**��
```csharp
"123456789.123456789".ToDecimal()  // ��ȷ����С��
```

### ����ֵת��

#### ToBoolean

```csharp
public static Boolean ToBoolean(this Object? value, Boolean defaultValue = false)
```

������ת��Ϊ����ֵ��

**֧�ֵ���ֵ**��`true`��`True`��`1`��`y`��`yes`��`on`��`enable`��`enabled`  
**֧�ֵļ�ֵ**��`false`��`False`��`0`��`n`��`no`��`off`��`disable`��`disabled`

**ʾ��**��
```csharp
"true".ToBoolean()               // true
"True".ToBoolean()               // true
"1".ToBoolean()                  // true
"yes".ToBoolean()                // true
"on".ToBoolean()                 // true
"enable".ToBoolean()             // true

"false".ToBoolean()              // false
"0".ToBoolean()                  // false
"no".ToBoolean()                 // false
"off".ToBoolean()                // false

"invalid".ToBoolean()            // false��Ĭ��ֵ��
"invalid".ToBoolean(true)        // true��ָ��Ĭ��ֵ��
```

### ʱ��ת��

#### ToDateTime

```csharp
public static DateTime ToDateTime(this Object? value)
public static DateTime ToDateTime(this Object? value, DateTime defaultValue)
```

������ת��Ϊʱ�����ڡ�

**֧�ֵĸ�ʽ**��
- ��׼����ʱ���ַ���
- `yyyy-M-d` ��ʽ
- `yyyy/M/d` ��ʽ
- `yyyyMMddHHmmss` ��ʽ
- `yyyyMMdd` ��ʽ
- Unix �루Int32��
- Unix ���루Int64���Զ��жϣ�
- UTC ��ǣ�ĩβ `Z` �� ` UTC`��

**ʾ��**��
```csharp
// �ַ���ת��
"2024-01-15".ToDateTime()
"2024-1-5".ToDateTime()          // ֧�ֵ�λ������
"2024/01/15".ToDateTime()
"20240115".ToDateTime()
"20240115120000".ToDateTime()
"2024-01-15 12:30:45".ToDateTime()
"2024-01-15T12:30:45Z".ToDateTime()  // UTC ʱ��

// Unix ʱ���ת��
1705276800.ToDateTime()          // Unix ��
1705276800000L.ToDateTime()      // Unix ���루�Զ��жϣ�
```

> **ע��**������תʱ��ʱ������ UTC �뱾��ʱ��ת�����������������У��豸����λ�ڲ�ͬʱ��������ͳһʹ�� UTC ʱ�䴫�����ת����

#### ToDateTimeOffset

```csharp
public static DateTimeOffset ToDateTimeOffset(this Object? value)
public static DateTimeOffset ToDateTimeOffset(this Object? value, DateTimeOffset defaultValue)
```

������ת��Ϊ��ʱ����ʱ�����ڡ�

### ʱ���ʽ��

#### ToFullString

```csharp
public static String ToFullString(this DateTime value, String? emptyValue = null)
public static String ToFullString(this DateTime value, Boolean useMillisecond, String? emptyValue = null)
```

��ʱ���ʽ��Ϊ `yyyy-MM-dd HH:mm:ss` ��׼��ʽ��

**����˵��**��
- `useMillisecond`���Ƿ�������룬��ʽΪ `yyyy-MM-dd HH:mm:ss.fff`
- `emptyValue`����ʱ��Ϊ `MinValue` ʱ��ʾ������ַ���

**ʾ��**��
```csharp
DateTime.Now.ToFullString()                    // "2024-01-15 12:30:45"
DateTime.Now.ToFullString(true)                // "2024-01-15 12:30:45.123"
DateTime.MinValue.ToFullString("")             // ""
DateTime.MinValue.ToFullString("N/A")          // "N/A"
```

#### Trim

```csharp
public static DateTime Trim(this DateTime value, String format = "s")
```

�ض�ʱ�侫�ȡ�

**��ʽ����**��
- `ns`�����뾫�ȣ�ʵ��Ϊ 100ns���� 1 tick��
- `us`��΢�뾫��
- `ms`�����뾫��
- `s`���뾫�ȣ�Ĭ�ϣ�
- `m`�����Ӿ���
- `h`��Сʱ����

**ʾ��**��
```csharp
var dt = new DateTime(2024, 1, 15, 12, 30, 45, 123);
dt.Trim("s")                     // 2024-01-15 12:30:45.000
dt.Trim("m")                     // 2024-01-15 12:30:00.000
dt.Trim("h")                     // 2024-01-15 12:00:00.000
dt.Trim("ms")                    // ��������
```

### �ֽڵ�λ��ʽ��

#### ToGMK

```csharp
public static String ToGMK(this Int64 value, String? format = null)
public static String ToGMK(this UInt64 value, String? format = null)
```

���ֽ�����ʽ��Ϊ�ɶ��ĵ�λ�ַ�����

**ʾ��**��
```csharp
1024L.ToGMK()                    // "1.00K"
1048576L.ToGMK()                 // "1.00M"
1073741824L.ToGMK()              // "1.00G"
1099511627776L.ToGMK()           // "1.00T"

// �Զ����ʽ
1536L.ToGMK("n1")                // "1.5K"
1536L.ToGMK("n0")                // "2K"
```

### �쳣����

#### GetTrue

```csharp
public static Exception GetTrue(this Exception ex)
```

��ȡ�쳣����ʵ�ڲ��쳣���Զ���� `AggregateException`��`TargetInvocationException`��`TypeInitializationException` �Ȱ�װ�쳣��

**ʾ��**��
```csharp
try
{
    // �����׳���װ�쳣�Ĵ���
}
catch (Exception ex)
{
    var realEx = ex.GetTrue();
    Console.WriteLine(realEx.Message);
}
```

#### GetMessage

```csharp
public static String GetMessage(this Exception ex)
```

��ȡ��ʽ�����쳣��Ϣ�����˵�����Ҫ�Ķ�ջ��Ϣ���� `System.Runtime.ExceptionServices` �ȣ���

## �Զ���ת��

ͨ���滻 `Utility.Convert` �����Զ�����������ת������Ϊ��

```csharp
public class MyConvert : DefaultConvert
{
    public override Int32 ToInt(Object? value, Int32 defaultValue)
    {
        // �Զ���ת���߼�
        if (value is MyCustomType mct)
            return mct.Value;
            
        return base.ToInt(value, defaultValue);
    }
}

// ȫ���滻
Utility.Convert = new MyConvert();
```

## ���ʵ��

### 1. ʼ���ṩ�������Ĭ��ֵ

```csharp
// �Ƽ�����ȷָ��Ĭ��ֵ
var port = config["Port"].ToInt(8080);
var timeout = config["Timeout"].ToInt(30);

// ���Ƽ���ʹ����ʽĬ��ֵ 0 ���ܵ�������
var port = config["Port"].ToInt();  // �������ȱʧ���˿�Ϊ 0
```

### 2. ʱ���ת��ע��ʱ��

```csharp
// �������������豸�ϱ� UTC ʱ���
var deviceTime = timestamp.ToDateTime();      // ����ʱ��ת��
var localTime = deviceTime.ToLocalTime();     // תΪ����ʱ��

// ����ʹ�� DateTimeOffset
var dto = timestamp.ToDateTimeOffset();
```

### 3. ������ʽ���ü򻯴���

```csharp
// ��ͳд��
Int32 value;
if (!Int32.TryParse(str, out value))
    value = defaultValue;

// NewLife д��
var value = str.ToInt(defaultValue);
```

## ����˵��

- ����ת��������Գ������ͣ�String��Int32 �ȣ������˿���·���Ż�
- �ַ���ת��ʹ�� `Span<T>` ���ⲻ��Ҫ���ڴ����
- ʱ���ʽ������ʹ�� `ToString()` ��ʽ���������ֶ�ƴ����������
- �ֽ�����ת��ֱ��ʹ�� `BitConverter`���޶��⿪��

## �������

- [�ַ�����չ StringHelper](string_helper-�ַ�����չStringHelper.md)
- [������չ IOHelper](io_helper-������չIOHelper.md)
