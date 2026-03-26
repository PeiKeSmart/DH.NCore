# �ַ�����չ StringHelper

## ����

`StringHelper` �� DH.NCore �е��ַ������������࣬�ṩ�˷ḻ���ַ�����չ�����������Ƚϡ���ȡ����֡�ƴ�ӡ��༭���������ȹ��ܣ���������ճ������е��ַ���������

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ֵ��ȫ**�����з���������ȷ���� null �Ϳ��ַ���
- **���Դ�Сд**���ṩ���ֺ��Դ�Сд�ıȽϷ���
- **��Чʵ��**��ʹ�� `StringBuilder` �ػ���`Span<T>` �ȼ����Ż�����
- **ģ��ƥ��**������ Levenshtein �༭����� LCS ������������㷨

## ���ٿ�ʼ

```csharp
using NewLife;

// ��ֵ�ж�
var isEmpty = "".IsNullOrEmpty();           // true
var isBlank = "  ".IsNullOrWhiteSpace();    // true

// ���Դ�Сд�Ƚ�
var equal = "Hello".EqualIgnoreCase("hello");  // true

// �ַ������
var arr = "1,2,3".SplitAsInt();             // [1, 2, 3]
var dic = "a=1;b=2".SplitAsDictionary();    // {a:1, b:2}

// �ַ���ƴ��
var str = new[] { 1, 2, 3 }.Join(",");      // "1,2,3"
```

## API �ο�

### ��ֵ�ж�

#### IsNullOrEmpty

```csharp
public static Boolean IsNullOrEmpty(this String? value)
```

�ж��ַ����Ƿ�Ϊ null ����ַ�����

**ʾ��**��
```csharp
String? s1 = null;
s1.IsNullOrEmpty()               // true
"".IsNullOrEmpty()               // true
" ".IsNullOrEmpty()              // false���ո���գ�
"hello".IsNullOrEmpty()          // false
```

#### IsNullOrWhiteSpace

```csharp
public static Boolean IsNullOrWhiteSpace(this String? value)
```

�ж��ַ����Ƿ�Ϊ null�����ַ�����������հ��ַ���

**ʾ��**��
```csharp
String? s1 = null;
s1.IsNullOrWhiteSpace()          // true
"".IsNullOrWhiteSpace()          // true
"   ".IsNullOrWhiteSpace()       // true
"\t\n".IsNullOrWhiteSpace()      // true
"hello".IsNullOrWhiteSpace()     // false
```

### �ַ����Ƚ�

#### EqualIgnoreCase

```csharp
public static Boolean EqualIgnoreCase(this String? value, params String?[] strs)
```

���Դ�Сд�Ƚ��ַ����Ƿ�������һ����ѡ�ַ�����ȡ�

**ʾ��**��
```csharp
"Hello".EqualIgnoreCase("hello")                    // true
"Hello".EqualIgnoreCase("HELLO", "World")           // true
"Hello".EqualIgnoreCase("World", "Test")            // false
```

#### StartsWithIgnoreCase

```csharp
public static Boolean StartsWithIgnoreCase(this String? value, params String?[] strs)
```

���Դ�Сд�ж��ַ����Ƿ�������һ����ѡǰ׺��ʼ��

**ʾ��**��
```csharp
"HelloWorld".StartsWithIgnoreCase("hello")          // true
"HelloWorld".StartsWithIgnoreCase("HELLO", "Hi")    // true
```

#### EndsWithIgnoreCase

```csharp
public static Boolean EndsWithIgnoreCase(this String? value, params String?[] strs)
```

���Դ�Сд�ж��ַ����Ƿ�������һ����ѡ��׺������

**ʾ��**��
```csharp
"HelloWorld".EndsWithIgnoreCase("world")            // true
"HelloWorld".EndsWithIgnoreCase("WORLD", "Test")    // true
```

### ͨ���ƥ��

#### IsMatch

```csharp
public static Boolean IsMatch(this String pattern, String input, StringComparison comparisonType = StringComparison.CurrentCulture)
```

ʹ��ͨ���ģʽƥ���ַ�����֧�� `*`��ƥ�����ⳤ�ȣ��� `?`��ƥ�䵥���ַ�����

**�ص�**��
- ���������ʽ���򵥡�����Ч
- ʱ�临�Ӷ� O(n) ~ O(n*m)
- ���蹹���������

**ʾ��**��
```csharp
"*.txt".IsMatch("document.txt")                     // true
"*.txt".IsMatch("document.doc")                     // false
"file?.txt".IsMatch("file1.txt")                    // true
"file?.txt".IsMatch("file12.txt")                   // false
"*".IsMatch("anything")                             // true��ƥ�����У�
"test*end".IsMatch("test123end")                    // true
```

### �ַ������

#### Split����չ���أ�

```csharp
public static String[] Split(this String? value, params String[] separators)
```

��ָ���ָ�������ַ������Զ����˿���Ŀ��

**ʾ��**��
```csharp
"a,b,,c".Split(",")              // ["a", "b", "c"]���Զ����˿��
"a;b,c".Split(",", ";")          // ["a", "b", "c"]
```

#### SplitAsInt

```csharp
public static Int32[] SplitAsInt(this String? value, params String[] separators)
```

����ַ�����ת��Ϊ�������飬Ĭ��ʹ�ö��źͷֺ���Ϊ�ָ�����

**�ص�**��
- �Զ����˿ո�
- �Զ�������Ч����
- �����ظ���

**ʾ��**��
```csharp
"1,2,3".SplitAsInt()             // [1, 2, 3]
"1, 2, 3".SplitAsInt()           // [1, 2, 3]���Զ�ȥ���ո�
"1;2;3".SplitAsInt()             // [1, 2, 3]��֧�ַֺţ�
"1,abc,3".SplitAsInt()           // [1, 3]��������Ч�
"1,1,2".SplitAsInt()             // [1, 1, 2]�������ظ���
```

#### SplitAsDictionary

```csharp
public static IDictionary<String, String> SplitAsDictionary(
    this String? value, 
    String nameValueSeparator = "=", 
    String separator = ";", 
    Boolean trimQuotation = false)
```

���ַ������Ϊ��ֵ���ֵ䡣

**����˵��**��
- `nameValueSeparator`����ֵ�ָ�����Ĭ�� `=`
- `separator`����Ŀ�ָ�����Ĭ�� `;`
- `trimQuotation`���Ƿ�ȥ��ֵ���˵�����

**ʾ��**��
```csharp
// �����÷�
"a=1;b=2".SplitAsDictionary()
// { "a": "1", "b": "2" }

// �Զ���ָ���
"a:1,b:2".SplitAsDictionary(":", ",")
// { "a": "1", "b": "2" }

// ȥ������
"name='test';value=\"123\"".SplitAsDictionary("=", ";", true)
// { "name": "test", "value": "123" }

// �޼���ʱʹ�����
"value1;key=value2".SplitAsDictionary()
// { "[0]": "value1", "key": "value2" }
```

> **��ʾ**�����ص��ֵ䲻���ִ�Сд��`StringComparer.OrdinalIgnoreCase`��

### �ַ���ƴ��

#### Join

```csharp
public static String Join(this IEnumerable value, String separator = ",")
public static String Join<T>(this IEnumerable<T> value, String separator = ",", Func<T, Object?>? func = null)
```

������Ԫ��ƴ��Ϊ�ַ�����

**ʾ��**��
```csharp
// �����÷�
new[] { 1, 2, 3 }.Join()         // "1,2,3"
new[] { 1, 2, 3 }.Join(";")      // "1;2;3"

// ʹ��ת������
var users = new[] { new { Name = "����" }, new { Name = "����" } };
users.Join(",", u => u.Name)     // "����,����"
```

#### Separate

```csharp
public static StringBuilder Separate(this StringBuilder sb, String separator)
```

�� `StringBuilder` ׷�ӷָ�����������Կ�ͷ����һ�ε��ò�׷�ӣ���

**ʾ��**��
```csharp
var sb = new StringBuilder();
sb.Separate(",").Append("a");    // "a"
sb.Separate(",").Append("b");    // "a,b"
sb.Separate(",").Append("c");    // "a,b,c"
```

### �ַ�����ȡ

#### Substring����չ���أ�

```csharp
public static String Substring(this String str, String? after, String? before = null, Int32 startIndex = 0, Int32[]? positions = null)
```

���ַ����н�ȡָ�����֮������ݡ�

**ʾ��**��
```csharp
// ��ȡ���֮�������
"Hello[World]End".Substring("[")            // "World]End"

// ��ȡ�������֮�������
"Hello[World]End".Substring("[", "]")       // "World"

// ��ȡ���֮ǰ������
"Hello[World]End".Substring(null, "[")      // "Hello"

// ��ȡƥ��λ��
var positions = new Int32[2];
"Hello[World]End".Substring("[", "]", 0, positions);
// positions[0] = 6��������ʼλ�ã�
// positions[1] = 11�����ݽ���λ�ã�
```

#### Cut

```csharp
public static String Cut(this String str, Int32 maxLength, String? pad = null)
```

����󳤶Ƚ�ȡ�ַ�������ָ������ַ���

**ʾ��**��
```csharp
"HelloWorld".Cut(8)              // "HelloWor"
"HelloWorld".Cut(8, "...")       // "Hello..."���ܳ��Ȳ�����8��
"Hi".Cut(8)                      // "Hi"�����㳤��ԭ�����أ�
```

#### TrimStart / TrimEnd

```csharp
public static String TrimStart(this String str, params String[] starts)
public static String TrimEnd(this String str, params String[] ends)
```

���ַ�����ͷ/��β�Ƴ�ָ�������ַ����������ִ�Сд��֧�ֶ��ƥ�䡣

**ʾ��**��
```csharp
"HelloHelloWorld".TrimStart("Hello")     // "World"���Ƴ�����ƥ���ǰ׺��
"WorldEndEnd".TrimEnd("End")             // "World"
```

#### CutStart / CutEnd

```csharp
public static String CutStart(this String str, params String[] starts)
public static String CutEnd(this String str, params String[] ends)
```

�Ƴ�ָ�����ַ�������֮ǰ/֮����������ݡ�

**ʾ��**��
```csharp
"path/to/file.txt".CutStart("/")         // "file.txt"
"path/to/file.txt".CutEnd("/")           // "path/to"
```

#### EnsureStart / EnsureEnd

```csharp
public static String EnsureStart(this String? str, String start)
public static String EnsureEnd(this String? str, String end)
```

ȷ���ַ�����ָ�����ݿ�ʼ/������

**ʾ��**��
```csharp
"world".EnsureStart("Hello")     // "Helloworld"
"Hello".EnsureStart("Hello")     // "Hello"���Ѵ��������ӣ�

"/api/users".EnsureEnd("/")      // "/api/users/"
"/api/users/".EnsureEnd("/")     // "/api/users/"
```

#### TrimInvisible

```csharp
public static String? TrimInvisible(this String? value)
```

�Ƴ��ַ����еĲ��ɼ� ASCII �����ַ���0-31 �� 127����

**ʾ��**��
```csharp
"Hello\x00World\x1F".TrimInvisible()     // "HelloWorld"
```

### ����ת��

#### GetBytes

```csharp
public static Byte[] GetBytes(this String? value, Encoding? encoding = null)
```

���ַ���ת��Ϊ�ֽ����飬Ĭ��ʹ�� UTF-8 ���롣

**ʾ��**��
```csharp
"Hello".GetBytes()                        // UTF-8 ������ֽ�����
"Hello".GetBytes(Encoding.ASCII)          // ASCII ����
"���".GetBytes(Encoding.UTF8)            // UTF-8 ��������
```

### ģ������

#### Levenshtein �༭����

```csharp
public static Int32 LevenshteinDistance(String str1, String str2)
public static String[] LevenshteinSearch(String key, String[] words)
```

���������ַ���֮��ı༭���루���롢ɾ�����滻���������ٴ�������

**ʾ��**��
```csharp
// ����༭����
StringHelper.LevenshteinDistance("kitten", "sitting")  // 3

// ģ������
var words = new[] { "apple", "application", "banana", "apply" };
StringHelper.LevenshteinSearch("appl", words)
// ["apple", "application", "apply"]
```

#### LCS �����������

```csharp
public static Int32 LCSDistance(String word, String[] keys)
public static String[] LCSSearch(String key, String[] words)
public static IEnumerable<T> LCSSearch<T>(this IEnumerable<T> list, String keys, Func<T, String> keySelector, Int32 count = -1)
```

��������������е�ģ���������ʺ������������顢�Զ���ȫ�ȳ�����

**ʾ��**��
```csharp
var words = new[] { "HelloWorld", "HelloKitty", "GoodBye" };
StringHelper.LCSSearch("Hello", words)
// ["HelloKitty", "HelloWorld"]

// ��������
var users = new[] { 
    new { Id = 1, Name = "����" },
    new { Id = 2, Name = "��С��" },
    new { Id = 3, Name = "����" }
};
users.LCSSearch("��", u => u.Name, 2)
// ������������С��
```

#### Match ģ��ƥ��

```csharp
public static IList<KeyValuePair<T, Double>> Match<T>(this IEnumerable<T> list, String keys, Func<T, String> keySelector)
public static IEnumerable<T> Match<T>(this IEnumerable<T> list, String keys, Func<T, String> keySelector, Int32 count, Double confidence = 0.5)
```

���������ʺ������ͷ���ģ��ƥ���㷨��

**ʾ��**��
```csharp
var products = new[] { "iPhone 15", "iPhone 15 Pro", "Samsung Galaxy" };
products.Match("iPhone", s => s, 2, 0.3)
// ["iPhone 15", "iPhone 15 Pro"]
```

### ����ת����

```csharp
public static void Speak(this String value)
public static void SpeakAsync(this String value)
```

����ϵͳ���������ʶ��ı����� Windows ƽ̨����

**ʾ��**��
```csharp
"��ã�����".Speak();       // ͬ���ʶ�
"��ã�����".SpeakAsync();  // �첽�ʶ�
```

## ���ʵ��

### 1. ʹ�ÿ�ֵ��ȫ�ķ���

```csharp
// �Ƽ���ʹ����չ����
if (str.IsNullOrEmpty()) return;

// ���Ƽ�����Ҫ���� null
if (str == null || str.Length == 0) return;
```

### 2. ���������ַ���

```csharp
// �����ַ�������
var connStr = "Server=localhost;Database=test;User=root;Password=123456";
var dic = connStr.SplitAsDictionary();
var server = dic["Server"];      // "localhost"
var database = dic["Database"];  // "test"
```

### 3. URL ��������

```csharp
var query = "name=test&age=18&tags=a,b,c";
var dic = query.SplitAsDictionary("=", "&");
var name = dic["name"];          // "test"
var tags = dic["tags"].Split(",");  // ["a", "b", "c"]
```

### 4. ʹ�� StringBuilder ��

```csharp
using NewLife.Collections;

// �ӳ��л�ȡ StringBuilder
var sb = Pool.StringBuilder.Get();
sb.Append("Hello");
sb.Separate(",").Append("World");

// �����ַ������黹����
var result = sb.Return(true);    // "Hello,World"
```

## ����˵��

- `IsNullOrEmpty` �� `IsNullOrWhiteSpace` ʹ�������Ż�
- �ַ�����ֺ�ƴ��ʹ�� `StringBuilder` �أ������ڴ����
- ͨ���ƥ��ʹ�õ�ָ������㷨�������������ʽ����
- �༭�����㷨��Զ��ַ����Ż�

## �������

- [����ת�� Utility](utility-����ת��Utility.md)
- [������չ IOHelper](io_helper-������չIOHelper.md)
- [·����չ PathHelper](path_helper-·����չPathHelper.md)
