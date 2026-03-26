# XML ���л�

## ����

DH.NCore �ṩ������ XML ���л��ͷ����л����ܣ�ͨ�� `XmlHelper` ��չ�������Է���ؽ��ж����� XML ��ת����֧���Զ�����ע�͡�����ģʽ��������ԣ��ر��ʺ������ļ�������

**�����ռ�**��`NewLife.Xml`����չ��������`NewLife.Serialization`������ʵ�֣�  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��� API**��`ToXml()` �� `ToXmlEntity<T>()` ��չ����
- **ע��֧��**���Զ����� `Description` �� `DisplayName` ������Ϊע��
- **����ģʽ**����ѡ���������л�Ϊ XML ���Զ���Ԫ��
- **�������**��֧��ָ�������ʽ
- **�ļ�����**��ֱ�����л����ļ�����ļ������л�

## ���ٿ�ʼ

### ���л�

```csharp
using NewLife.Xml;

public class AppConfig
{
    public String Name { get; set; }
    public Int32 Port { get; set; }
    public Boolean Debug { get; set; }
}

var config = new AppConfig
{
    Name = "MyApp",
    Port = 8080,
    Debug = true
};

// ���л�Ϊ XML �ַ���
var xml = config.ToXml();
```

**���**��
```xml
<?xml version="1.0" encoding="utf-8"?>
<AppConfig>
  <Name>MyApp</Name>
  <Port>8080</Port>
  <Debug>true</Debug>
</AppConfig>
```

### �����л�

```csharp
using NewLife.Xml;

var xml = """
<?xml version="1.0" encoding="utf-8"?>
<AppConfig>
  <Name>MyApp</Name>
  <Port>8080</Port>
  <Debug>true</Debug>
</AppConfig>
""";

var config = xml.ToXmlEntity<AppConfig>();
Console.WriteLine(config.Name);  // MyApp
```

## API �ο�

### ToXml - ���л�

```csharp
// �������л�
public static String ToXml(this Object obj, Encoding? encoding = null, 
    Boolean attachComment = false, Boolean useAttribute = false)

// ��������
public static String ToXml(this Object obj, Encoding encoding, 
    Boolean attachComment, Boolean useAttribute, Boolean omitXmlDeclaration)

// ���л�����
public static void ToXml(this Object obj, Stream stream, Encoding? encoding = null, 
    Boolean attachComment = false, Boolean useAttribute = false)

// ���л����ļ�
public static void ToXmlFile(this Object obj, String file, Encoding? encoding = null, 
    Boolean attachComment = true)
```

**����˵��**��
- `encoding`�������ʽ��Ĭ�� UTF-8
- `attachComment`���Ƿ񸽼�ע�ͣ�ʹ�� Description/DisplayName��
- `useAttribute`���Ƿ�ʹ�� XML ����ģʽ
- `omitXmlDeclaration`���Ƿ�ʡ�� XML ����

### ToXmlEntity - �����л�

```csharp
// ���ַ��������л�
public static TEntity? ToXmlEntity<TEntity>(this String xml) where TEntity : class

// ���������л�
public static TEntity? ToXmlEntity<TEntity>(this Stream stream, Encoding? encoding = null)

// ���ļ������л�
public static TEntity? ToXmlFileEntity<TEntity>(this String file, Encoding? encoding = null)
```

## ʹ�ó���

### 1. �����ļ�

```csharp
using System.ComponentModel;
using NewLife.Xml;

public class DatabaseConfig
{
    [Description("���ݿ��������ַ")]
    public String Server { get; set; } = "localhost";
    
    [Description("���ݿ�˿�")]
    public Int32 Port { get; set; } = 3306;
    
    [Description("���ݿ�����")]
    public String Database { get; set; } = "mydb";
    
    [Description("�û���")]
    public String User { get; set; } = "root";
    
    [Description("���ӳ�ʱ���룩")]
    public Int32 Timeout { get; set; } = 30;
}

// �������ã���ע�ͣ�
var config = new DatabaseConfig();
config.ToXmlFile("db.config", attachComment: true);

// ��������
var loaded = "db.config".ToXmlFileEntity<DatabaseConfig>();
```

**���ɵ� XML**��
```xml
<?xml version="1.0" encoding="utf-8"?>
<DatabaseConfig>
  <!--���ݿ��������ַ-->
  <Server>localhost</Server>
  <!--���ݿ�˿�-->
  <Port>3306</Port>
  <!--���ݿ�����-->
  <Database>mydb</Database>
  <!--�û���-->
  <User>root</User>
  <!--���ӳ�ʱ���룩-->
  <Timeout>30</Timeout>
</DatabaseConfig>
```

### 2. ����ģʽ���

```csharp
public class Item
{
    public Int32 Id { get; set; }
    public String Name { get; set; }
    public Decimal Price { get; set; }
}

var item = new Item { Id = 1, Name = "��ƷA", Price = 99.9M };

// Ԫ��ģʽ��Ĭ�ϣ�
var xml1 = item.ToXml();
// <Item><Id>1</Id><Name>��ƷA</Name><Price>99.9</Price></Item>

// ����ģʽ
var xml2 = item.ToXml(useAttribute: true);
// <Item Id="1" Name="��ƷA" Price="99.9" />
```

### 3. ���Ӷ���

```csharp
public class Order
{
    public Int32 Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class Customer
{
    public String Name { get; set; }
    public String Phone { get; set; }
}

public class OrderItem
{
    public String ProductName { get; set; }
    public Int32 Quantity { get; set; }
    public Decimal Price { get; set; }
}

var order = new Order
{
    Id = 1001,
    CreateTime = DateTime.Now,
    Customer = new Customer { Name = "����", Phone = "13800138000" },
    Items = new List<OrderItem>
    {
        new() { ProductName = "��ƷA", Quantity = 2, Price = 50 },
        new() { ProductName = "��ƷB", Quantity = 1, Price = 100 }
    }
};

var xml = order.ToXml();
```

### 4. ʡ�� XML ����

```csharp
// ʡ�� <?xml version="1.0" encoding="utf-8"?>
var xml = obj.ToXml(Encoding.UTF8, false, false, true);
```

### 5. �ֵ����л�

```csharp
// �ַ����ֵ����ֱ�����л�
var dict = new Dictionary<String, String>
{
    ["Key1"] = "Value1",
    ["Key2"] = "Value2"
};

dict.ToXmlFile("settings.xml");
```

## Xml �ࣨ�߼��÷���

������Ҫ����ϸ���Ƶĳ���������ֱ��ʹ�� `Xml` �ࣺ

```csharp
using NewLife.Serialization;

// ���л�
var xml = new Xml
{
    Stream = stream,
    Encoding = Encoding.UTF8,
    UseAttribute = false,
    UseComment = true,
    EnumString = true  // ö��ʹ���ַ���
};
xml.Write(obj);

// �����л�
var xml = new Xml
{
    Stream = stream,
    Encoding = Encoding.UTF8
};
var result = xml.Read(typeof(MyClass));
```

### Xml ������

```csharp
public class Xml
{
    /// <summary>ʹ���������</summary>
    public Boolean UseAttribute { get; set; }
    
    /// <summary>ʹ��ע��</summary>
    public Boolean UseComment { get; set; }
    
    /// <summary>ö��ʹ���ַ�����Ĭ��true</summary>
    public Boolean EnumString { get; set; }
    
    /// <summary>XMLд������</summary>
    public XmlWriterSettings Setting { get; set; }
}
```

## ����֧��

### XmlRoot - ��Ԫ������

```csharp
[XmlRoot("config")]
public class AppConfig
{
    public String Name { get; set; }
}

// ��� <config><Name>...</Name></config>
```

### XmlElement - Ԫ������

```csharp
public class User
{
    [XmlElement("user_name")]
    public String Name { get; set; }
}
```

### XmlAttribute - ���Ϊ����

```csharp
public class Item
{
    [XmlAttribute]
    public Int32 Id { get; set; }
    
    public String Name { get; set; }
}

// ��� <Item Id="1"><Name>...</Name></Item>
```

### XmlIgnore - �����ֶ�

```csharp
public class User
{
    public String Name { get; set; }
    
    [XmlIgnore]
    public String Password { get; set; }  // �����л�
}
```

## ���ʵ��

### 1. �����ļ�ʹ��ע��

```csharp
// ����ʱ����ע��
config.ToXmlFile("app.config", attachComment: true);

// ʹ�� Description ��������˵��
[Description("Ӧ�����ƣ�������־��ʶ")]
public String AppName { get; set; }
```

### 2. �ļ�����ע������

```csharp
// ToXmlFile ���Զ�����Ŀ¼
config.ToXmlFile("Config/app.xml");

// ����ļ��Ƿ����
if (File.Exists(file))
{
    var config = file.ToXmlFileEntity<AppConfig>();
}
```

### 3. ����һ����

```csharp
// ����ͼ���ʹ����ͬ����
var encoding = Encoding.UTF8;
config.ToXmlFile("config.xml", encoding);
var loaded = "config.xml".ToXmlFileEntity<AppConfig>(encoding);
```

## �� JSON �Ա�

| ���� | XML | JSON |
|------|-----|------|
| �ɶ��� | ��ע�͸����� | ������ |
| ��� | �ϴ� | ��С |
| ע��֧�� | ԭ��֧�� | ��֧�� |
| �����ļ� | ? �Ƽ� | ? ���� |
| API ���� | ? ���Ƽ� | ? �Ƽ� |

## �������

- [JSON ���л�](json-JSON���л�.md)
- [����ϵͳ Config](config-����ϵͳConfig.md)
- [���������л� Binary](binary-���������л�Binary.md)
