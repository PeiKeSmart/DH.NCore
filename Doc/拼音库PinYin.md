# ƴ���� PinYin

## ����

`PinYin` �� DH.NCore �еĺ���ƴ��ת�������࣬�ṩ��Ч�ĺ���תƴ�����ܡ�֧�� GB2312 һ���Ͷ������֣��ܹ���ȡ���ֵ�ȫƴ��ƴ������ĸ���������������������뷨�ȳ�����

**�����ռ�**��`NewLife.Common`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **������**������ GB2312 �������λ���㷨��������ش����ֵ��ļ�
- **ȫ����֧��**������ GB2312 һ�����֣�3755�����Ͷ������֣�3008����
- **�������**��֧��ȫƴ������ĸ������ƴ���ȶ�����ʽ
- **������**�����ⲿ���������㷨ʵ��
- **���⴦��**��֧��"����"�ȶ����ֵĳ�������

## ���ٿ�ʼ

```csharp
using NewLife.Common;

// ��ȡ����ƴ��
var py = PinYin.Get('��');           // "Zhong"

// ��ȡ�ַ���ȫƴ
var fullPy = PinYin.Get("������");    // "XinShengMing"

// ��ȡƴ������ĸ
var first = PinYin.GetFirst("������"); // "XSM"

// ��ȡƴ������
var arr = PinYin.GetAll("���");      // ["Ni", "Hao"]
```

## API �ο�

### Get������ƴ����

```csharp
public static String Get(Char ch)
```

��ȡ�������ֵ�ƴ����

**����˵��**��
- `ch`��Ҫת�����ַ�

**����ֵ**��
- ���ַ�������ĸ��д��ƴ������ "Zhong"��
- �����ַ�����㡢�������ַ�ԭ������
- �޷�ʶ��ĺ��ַ��ؿ��ַ���

**ʾ��**��
```csharp
PinYin.Get('��')     // "Zhong"
PinYin.Get('��')     // "Guo"
PinYin.Get('A')      // "A"
PinYin.Get('��')     // "��"
PinYin.Get('��')     // "��"
```

### Get���ַ���ȫƴ��

```csharp
public static String Get(String str)
```

��ȡ�ַ���������ƴ��������ƴ��ֱ�����ӡ�

**ʾ��**��
```csharp
PinYin.Get("�й�")           // "ZhongGuo"
PinYin.Get("�������Ŷ�")     // "XinShengMingTuanDui"
PinYin.Get("Hello����")      // "HelloShiJie"
```

### GetAll

```csharp
public static String[] GetAll(String str)
```

��ȡ�ַ�����ÿ���ַ���ƴ���������ַ������顣

**���⴦��**��
- "����" ���⴦��Ϊ ["Chong", "Qing"]

**ʾ��**��
```csharp
PinYin.GetAll("���")        // ["Ni", "Hao"]
PinYin.GetAll("����")        // ["Chong", "Qing"]
PinYin.GetAll("ABC")         // ["A", "B", "C"]
PinYin.GetAll("Hello")       // ["H", "e", "l", "l", "o"]
```

### GetFirst��ƴ������ĸ��

```csharp
public static Char GetFirst(Char ch)
public static String GetFirst(String str)
```

��ȡ���ֻ��ַ�����ƴ������ĸ��

**ʾ��**��
```csharp
// ��������ĸ
PinYin.GetFirst('��')        // 'Z'
PinYin.GetFirst('��')        // 'G'
PinYin.GetFirst('A')         // 'A'

// �ַ�������ĸ
PinYin.GetFirst("������")    // "XSM"
PinYin.GetFirst("�й�")      // "ZG"
PinYin.GetFirst("Hello")     // "Hello"
```

## ʹ�ó���

### 1. ����ƥ��

```csharp
public class UserService
{
    /// <summary>����ƴ������ĸ�����û�</summary>
    public List<User> SearchByPinyin(List<User> users, String keyword)
    {
        var upperKeyword = keyword.ToUpper();
        
        return users.Where(u =>
        {
            // ȫ��ƥ��
            if (u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
            
            // ƴ������ĸƥ��
            var firstLetters = PinYin.GetFirst(u.Name);
            return firstLetters.Contains(upperKeyword, StringComparison.OrdinalIgnoreCase);
            
        }).ToList();
    }
}

// ʹ��ʾ��
var users = new List<User>
{
    new User { Name = "����" },
    new User { Name = "����" },
    new User { Name = "����" }
};

var result = service.SearchByPinyin(users, "ZS");  // �ҵ� "����"
var result2 = service.SearchByPinyin(users, "LS"); // �ҵ� "����"
```

### 2. ��ƴ������

```csharp
public class ProductSorter
{
    /// <summary>��ƴ��������Ʒ����</summary>
    public List<Product> SortByPinyin(List<Product> products)
    {
        return products
            .OrderBy(p => PinYin.Get(p.Name))
            .ToList();
    }
}

// ʹ��ʾ��
var products = new List<Product>
{
    new Product { Name = "ƻ��" },
    new Product { Name = "�㽶" },
    new Product { Name = "����" }
};

var sorted = sorter.SortByPinyin(products);
// ������������(ChengZi) -> ƻ��(PingGuo) -> �㽶(XiangJiao)
```

### 3. ����ƴ������

```csharp
public class ContactIndexer
{
    /// <summary>������ϵ��ƴ������</summary>
    public Dictionary<Char, List<Contact>> BuildIndex(List<Contact> contacts)
    {
        var index = new Dictionary<Char, List<Contact>>();
        
        foreach (var contact in contacts)
        {
            var firstLetter = PinYin.GetFirst(contact.Name[0]);
            
            if (!index.ContainsKey(firstLetter))
                index[firstLetter] = new List<Contact>();
            
            index[firstLetter].Add(contact);
        }
        
        return index;
    }
}

// ʹ��ʾ��
var contacts = new List<Contact>
{
    new Contact { Name = "����" },
    new Contact { Name = "����" },
    new Contact { Name = "����" }
};

var index = indexer.BuildIndex(contacts);
// index['Z'] = [����, ����]
// index['L'] = [����]
```

### 4. ���뷨��ʾ

```csharp
public class InputSuggestion
{
    private readonly List<String> _words;
    
    public InputSuggestion(List<String> words)
    {
        _words = words;
    }
    
    /// <summary>���������ȡ�����</summary>
    public List<String> GetSuggestions(String input)
    {
        if (input.IsNullOrEmpty()) return new List<String>();
        
        var upperInput = input.ToUpper();
        
        return _words
            .Where(w =>
            {
                // ֧������ĸƥ��
                var first = PinYin.GetFirst(w);
                if (first.StartsWith(upperInput, StringComparison.OrdinalIgnoreCase))
                    return true;
                
                // ֧��ȫƴƥ��
                var full = PinYin.Get(w);
                return full.StartsWith(upperInput, StringComparison.OrdinalIgnoreCase);
            })
            .Take(10)
            .ToList();
    }
}

// ʹ��ʾ��
var words = new List<String> { "������", "�����", "��������", "���տ���" };
var suggester = new InputSuggestion(words);

suggester.GetSuggestions("XS");   // ["������", "��������"]
suggester.GetSuggestions("Xin");  // ["������", "�����", "��������"]
```

### 5. ���ݿ�洢�Ż�

```csharp
public class User
{
    public Int32 Id { get; set; }
    public String Name { get; set; }
    
    /// <summary>����ƴ��������������</summary>
    public String NamePinyin { get; set; }
    
    /// <summary>��������ĸ������������</summary>
    public String NameFirst { get; set; }
    
    /// <summary>����ǰ�Զ�����ƴ���ֶ�</summary>
    public void BeforeSave()
    {
        NamePinyin = PinYin.Get(Name);
        NameFirst = PinYin.GetFirst(Name);
    }
}

// ���ݿ��ѯʱ������ƴ���ֶμ�������
// SELECT * FROM Users WHERE NameFirst LIKE 'ZS%'
// SELECT * FROM Users WHERE NamePinyin LIKE 'Zhang%'
```

## ����ϸ��

### �������

PinYin ����� GB2312 ����ʵ�֣�

1. **һ������**���� 3755 ������ƴ��˳������
2. **��������**���� 3008 ���������ױʻ�˳������
3. **���⺺��**������ GB2312 ��Χ�ĳ��ú��ֵ�������

### �㷨ԭ��

```
�ַ� �� GB2312���� �� ��λ�� �� ƴ��ӳ��
```

1. ������ת��Ϊ GB2312 �ֽ�
2. ������λ�루���ֽ�����ټ�ƫ�ƣ�
3. һ������ͨ������ӳ����ٶ�λƴ��
4. ��������ͨ��������һ�ȡƴ��

### �����ص�

- **���ֵ����**���㷨ֱ�Ӽ��㣬����������
- **O(1) ���Ӷ�**��һ������ͨ���ֿ��㷨���ٶ�λ
- **�ڴ�ռ��С**�����洢ƴ����������

## ����˵��

### ������

Ŀǰ��֧�ֶ������������жϣ�������ȡ���ö�����

```csharp
PinYin.Get('��')     // "Zhong"������ "Chong"��
PinYin.Get("����")   // "ChongQing"�����⴦����
PinYin.Get("��Ҫ")   // "ZhongYao"�������ö�����
```

### ��Ƨ��

���� GB2312 ��Χ����Ƨ�ֿ����޷�ת����

```csharp
PinYin.Get('?')     // "?"���޷�ʶ��ԭ�����أ�
```

### ������

��֧�ַ�����ת������Ҫ��ת���壺

```csharp
PinYin.Get('��')     // "��"���޷�ʶ��
PinYin.Get('��')     // "Guo"������������
```

## ���ʵ��

### 1. Ԥ����ƴ���ֶ�

```csharp
// �Ƽ������ʱԤ�ȼ���ƴ��
user.NamePinyin = PinYin.Get(user.Name);
user.NameFirst = PinYin.GetFirst(user.Name);
db.Save(user);

// ���Ƽ���ÿ�β�ѯʱʵʱ����
var users = db.Users.Where(u => PinYin.GetFirst(u.Name) == "ZS");
```

### 2. �����������

```csharp
// �Ƽ���ͬʱ֧��ԭ�ĺ�ƴ������
public List<User> Search(String keyword)
{
    return users.Where(u =>
        u.Name.Contains(keyword) ||
        u.NamePinyin.Contains(keyword.ToUpper()) ||
        u.NameFirst.Contains(keyword.ToUpper())
    ).ToList();
}
```

### 3. ���泣��ת��

```csharp
// ���ڸ�Ƶת���Ĵʻ㣬�ɿ��ǻ���
private static readonly ConcurrentDictionary<String, String> _cache = new();

public static String GetCached(String str)
{
    return _cache.GetOrAdd(str, s => PinYin.Get(s));
}
```

## �������

- [�ַ�����չ StringHelper](string_helper-�ַ�����չStringHelper.md)
- [����ת�� Utility](utility-����ת��Utility.md)
