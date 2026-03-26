# ���������л� Binary

## ����

`Binary` �� DH.NCore �еĸ����ܶ��������л��������ڽ��������л�Ϊ���յĶ����Ƹ�ʽ��Ӷ��������ݷ����л�Ϊ�����ر��ʺ�����ͨ�š�Э����������ݴ洢�ȶ����ܺ�����нϸ�Ҫ��ĳ�����

**�����ռ�**��`NewLife.Serialization`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **������**��ֱ�Ӳ����ֽ��������м��ʽת��
- **���ո�ʽ**��֧��7λ�䳤���������������������
- **�ֽ������**��֧�ִ��/С���ֽ���
- **Э��֧��**��֧�� `FieldSize` ���Զ����ֶδ�С
- **�汾����**��֧��Э��汾����
- **��չ��ǿ**���������Զ��崦����

## ���ٿ�ʼ

### ���л�

```csharp
using NewLife.Serialization;

public class User
{
    public Int32 Id { get; set; }
    public String Name { get; set; }
    public Int32 Age { get; set; }
}

var user = new User { Id = 1, Name = "����", Age = 25 };

// �������л�
var packet = Binary.FastWrite(user);
var bytes = packet.ToArray();

// ��ʹ����
using var ms = new MemoryStream();
Binary.FastWrite(user, ms);
```

### �����л�

```csharp
using NewLife.Serialization;

var bytes = /* ���������� */;

// ���ٷ����л�
using var ms = new MemoryStream(bytes);
var user = Binary.FastRead<User>(ms);
```

## API �ο�

### Binary ��

#### ����

```csharp
/// <summary>ʹ��7λ����������Ĭ��false��ʹ��</summary>
public Boolean EncodeInt { get; set; }

/// <summary>С���ֽ���Ĭ��false���</summary>
public Boolean IsLittleEndian { get; set; }

/// <summary>ʹ��ָ����С��FieldSizeAttribute���ԡ�Ĭ��false</summary>
public Boolean UseFieldSize { get; set; }

/// <summary>��С���ȡ���ѡ0/1/2/4��Ĭ��0��ʾѹ����������</summary>
public Int32 SizeWidth { get; set; }

/// <summary>�����ַ���ʱ���Ƿ������ͷ��0�ֽڡ�Ĭ��false</summary>
public Boolean TrimZero { get; set; }

/// <summary>Э��汾������֧�ֶ�汾Э�����л�</summary>
public String? Version { get; set; }

/// <summary>ʹ��������ʱ���ʽ��������ʽʹ��8���ֽڱ����������Ĭ��false</summary>
public Boolean FullTime { get; set; }

/// <summary>�ܵ��ֽ�������ȡ��д��</summary>
public Int64 Total { get; set; }
```

#### FastWrite - �������л�

```csharp
// ���л�Ϊ���ݰ�
public static IPacket FastWrite(Object value, Boolean encodeInt = true)

// ���л�����
public static Int64 FastWrite(Object value, Stream stream, Boolean encodeInt = true)
```

**����**��
- `value`��Ҫ���л��Ķ���
- `encodeInt`���Ƿ�ʹ��7λ�䳤��������
- `stream`��Ŀ����

**ʾ��**��
```csharp
// ���� IPacket
var packet = Binary.FastWrite(obj);
var bytes = packet.ToArray();

// д����
using var ms = new MemoryStream();
var length = Binary.FastWrite(obj, ms);
```

#### FastRead - ���ٷ����л�

```csharp
public static T? FastRead<T>(Stream stream, Boolean encodeInt = true)
```

**ʾ��**��
```csharp
using var ms = new MemoryStream(bytes);
var obj = Binary.FastRead<MyClass>(ms);
```

### �����÷�

```csharp
// �������л���
var bn = new Binary
{
    EncodeInt = true,           // ʹ��7λ����
    IsLittleEndian = true,      // С���ֽ���
    UseFieldSize = true         // ���� FieldSize ����
};

// ������
bn.Stream = new MemoryStream();

// д������
bn.Write(obj);

// ��ȡ���
var bytes = bn.GetBytes();
```

```csharp
// �����л�
var bn = new Binary
{
    Stream = new MemoryStream(bytes),
    EncodeInt = true
};

var obj = bn.Read<MyClass>();
```

## IAccessor �ӿ�

������Ҫ�Զ������л��߼������ͣ�����ʵ�� `IAccessor` �ӿڣ�

```csharp
public interface IAccessor
{
    /// <summary>����������ȡ</summary>
    Boolean Read(Stream stream, Object context);
    
    /// <summary>д��������</summary>
    Boolean Write(Stream stream, Object context);
}
```

**ʾ��**��
```csharp
public class CustomPacket : IAccessor
{
    public Byte Header { get; set; }
    public Int16 Length { get; set; }
    public Byte[] Data { get; set; }
    public Byte Checksum { get; set; }
    
    public Boolean Read(Stream stream, Object context)
    {
        var bn = context as Binary;
        
        Header = bn.ReadByte();
        Length = bn.Read<Int16>();
        Data = bn.ReadBytes(Length);
        Checksum = bn.ReadByte();
        
        return true;
    }
    
    public Boolean Write(Stream stream, Object context)
    {
        var bn = context as Binary;
        
        bn.Write(Header);
        bn.Write((Int16)Data.Length);
        bn.Write(Data);
        bn.Write(Checksum);
        
        return true;
    }
}
```

## FieldSize ����

`FieldSizeAttribute` ����ָ���ֶεĹ̶���С����������ֶΣ�

```csharp
public class Protocol
{
    public Byte Header { get; set; }
    
    [FieldSize(2)]  // �̶�2�ֽ�
    public Int16 Length { get; set; }
    
    [FieldSize("Length")]  // ��С�� Length �ֶξ���
    public Byte[] Body { get; set; }
    
    [FieldSize(4)]  // �̶�4�ֽ��ַ���
    public String Code { get; set; }
}
```

## ʹ�ó���

### 1. ����Э�����

```csharp
public class TcpMessage
{
    public Byte Start { get; set; } = 0x7E;
    public UInt16 MessageId { get; set; }
    public UInt16 BodyLength { get; set; }
    
    [FieldSize("BodyLength")]
    public Byte[] Body { get; set; }
    
    public Byte Checksum { get; set; }
    public Byte End { get; set; } = 0x7E;
}

// ����
var bn = new Binary(stream) { IsLittleEndian = false, UseFieldSize = true };
var msg = bn.Read<TcpMessage>();

// ����
var msg = new TcpMessage { MessageId = 0x0001, Body = data };
msg.BodyLength = (UInt16)data.Length;
msg.Checksum = CalculateChecksum(msg);
var packet = Binary.FastWrite(msg);
```

### 2. JT/T808 Э��

```csharp
public class JT808Message
{
    public UInt16 MsgId { get; set; }
    public UInt16 MsgAttr { get; set; }
    
    [FieldSize(6)]  // 2011��6�ֽڣ�2019��10�ֽ�
    public String Phone { get; set; }
    
    public UInt16 SeqNo { get; set; }
    public Byte[] Body { get; set; }
}

// 2011�汾
var bn = new Binary { UseFieldSize = true, Version = "2011" };
var msg = bn.Read<JT808Message>();

// 2019�汾
var bn = new Binary { UseFieldSize = true, Version = "2019" };
```

### 3. ���ݴ洢

```csharp
public class Record
{
    public Int64 Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Byte[] Data { get; set; }
}

// ���浽�ļ�
using var fs = File.Create("data.bin");
foreach (var record in records)
{
    Binary.FastWrite(record, fs);
}

// ���ļ�����
using var fs = File.OpenRead("data.bin");
var list = new List<Record>();
while (fs.Position < fs.Length)
{
    var record = Binary.FastRead<Record>(fs);
    list.Add(record);
}
```

### 4. ���������л�

```csharp
// ���� Binary ʵ������Ƶ������
var bn = new Binary { EncodeInt = true };

foreach (var item in items)
{
    bn.Stream = new MemoryStream();
    bn.Write(item);
    var bytes = bn.GetBytes();
    // ���� bytes...
}
```

## 7λ�䳤����

`EncodeInt = true` ʱ������ʹ��7λ�䳤���룬����������С��ֵ�Ĵ洢�ռ䣺

| ֵ��Χ | �ֽ��� |
|--------|--------|
| 0 ~ 127 | 1 �ֽ� |
| 128 ~ 16383 | 2 �ֽ� |
| 16384 ~ 2097151 | 3 �ֽ� |
| 2097152 ~ 268435455 | 4 �ֽ� |
| ���� | 5 �ֽ� |

```csharp
// ���ñ䳤����
var bn = new Binary { EncodeInt = true };
bn.Stream = new MemoryStream();
bn.Write(100);      // 1�ֽ�
bn.Write(1000);     // 2�ֽ�
bn.Write(100000);   // 3�ֽ�
```

## �ֽ���

```csharp
// ����ֽ��������ֽ���Ĭ�ϣ�
var bn = new Binary { IsLittleEndian = false };
bn.Write((Int32)0x12345678);  // ���: 12 34 56 78

// С���ֽ���Intel x86��
var bn = new Binary { IsLittleEndian = true };
bn.Write((Int32)0x12345678);  // ���: 78 56 34 12
```

## ���ʵ��

### 1. Э�����ͳһ����

```csharp
public static class BinaryConfig
{
    public static Binary CreateReader(Stream stream) => new Binary(stream)
    {
        EncodeInt = false,
        IsLittleEndian = false,
        UseFieldSize = true
    };
    
    public static Binary CreateWriter() => new Binary
    {
        EncodeInt = false,
        IsLittleEndian = false,
        UseFieldSize = true
    };
}
```

### 2. ������

```csharp
var bn = new Binary(stream);
try
{
    var obj = bn.Read<MyClass>();
}
catch (EndOfStreamException)
{
    // ���ݲ�����
}
```

### 3. ���ʣ������

```csharp
var bn = new Binary(stream);

// ����Ƿ����㹻����
if (bn.CheckRemain(10))  // ������Ҫ10�ֽ�
{
    var data = bn.ReadBytes(10);
}
```

## ���ܶԱ�

| ���л���ʽ | ��� | �ٶ� | ���ó��� |
|-----------|------|------|---------|
| Binary | ��С | ��� | Э�顢�洢 |
| JSON | �е� | �е� | API������ |
| XML | ��� | ���� | ���á��ĵ� |

## �������

- [JSON ���л�](json-JSON���л�.md)
- [XML ���л�](xml-XML���л�.md)
- [���ݰ� IPacket](packet-���ݰ�IPacket.md)
