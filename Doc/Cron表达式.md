# Cron����ʽ

## ����

`NewLife.Threading.Cron` ��һ���������� Cron ����ʽ������ƥ�����������ж�ĳ��ʱ����Ƿ�������򣬲��ܼ�����һ��/��һ�ε�ִ��ʱ�䡣

���׼ Cron ��ȣ�NewLife.Cron ����������࣬�ʺ��ڶ�ʱ���񡢵���ϵͳ��ʹ�á�

**�����ռ�**: `NewLife.Threading`  
**Դ��**: [DH.NCore/Threading/Cron.cs](https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Threading/Cron.cs)

---

## ��������

### �����ͽ���

```csharp
using NewLife.Threading;

// ��ʽ1������ʱ�������ʽ
var cron = new Cron("0 0 2 * * 1-5");

// ��ʽ2���ȴ��������
var cron2 = new Cron();
cron2.Parse("*/5 * * * * *");
```

### �ж�ʱ���Ƿ�ƥ��

```csharp
var cron = new Cron("0 0 2 * * 1-5");  // ÿ���������賿2��

// �жϵ�ǰʱ���Ƿ���ϱ���ʽ
if (cron.IsTime(DateTime.Now))
{
    Console.WriteLine("������ִ��ʱ��");
}

// �ж��ض�ʱ��
var time = new DateTime(2025, 1, 6, 2, 0, 0);  // 2025��1��6��(��һ) 2��
if (cron.IsTime(time))
{
    Console.WriteLine("��ʱ�����Ϲ���");
}
```

### ������һ��ִ��ʱ��

```csharp
var cron = new Cron("0 0 2 * * *");  // ÿ���賿2��
var next = cron.GetNext(DateTime.Now);
Console.WriteLine($"��һ��ִ��ʱ�䣺{next}");

// ������һ��ִ��ʱ��
var prev = cron.GetPrevious(DateTime.Now);
Console.WriteLine($"��һ��ִ��ʱ�䣺{prev}");
```

---

## ����ʽ�﷨

### ����ʽ�ṹ

Cron ����ʽ�ɿո�ָ��� **6 ���ֶ�**��ɣ���7���ֶ�"��"�ݲ�֧�֣���

```
�� �� ʱ �� �� ����
```

| �ֶ� | ��Χ | ˵�� |
|-----|------|------|
| �� | 0-59 | ���� |
| �� | 0-59 | ������ |
| ʱ | 0-23 | Сʱ����24Сʱ�ƣ� |
| �� | 1-31 | ÿ�µĵڼ��� |
| �� | 1-12 | �·� |
| ���� | 0-6 | ���ڼ���0=���գ�1=��һ��...��6=������ |

**ʾ��**��
```
0 30 8 * * 1-5     // ÿ�������� 8:30
0 0 */2 * * *      // ÿ2Сʱ����
0 0 0 1 * *        // ÿ��1���賿
*/10 * * * * *     // ÿ10��
```

### ֧�ֵ��﷨

#### 1. ͨ��� `*`

��ʾ���п��ܵ�ֵ��

```
* * * * * *        // ÿ��
0 * * * * *        // ÿ���ӵ�0��
0 0 * * * *        // ÿСʱ��0��0��
```

#### 2. ռλ�� `?`

��ָ��ֵ��ʵ���ϵȼ��� `*`����Ҫ���ڼ��� Quartz ������ Cron ʵ�֡�

```
0 0 0 ? * 1        // ÿ��һ�賿�����ڲ�ָ����
```

#### 3. ö�� `a,b,c`

�г����ָ��ֵ��

```
0 0 0 1,15 * *     // ÿ��1�ź�15���賿
0 0 8,12,18 * * *  // ÿ��8�㡢12�㡢18��
```

#### 4. ��Χ `a-b`

��ʾһ��������Χ�������䣩��

```
0 0 2 * * 1-5      // ��һ�������賿2��
0 0 9-17 * * *     // ÿ��9�㵽17�㣨ÿСʱ��
```

#### 5. ���� `*/n`��`a/n`��`a-b/n`

��ʾ����һ��������ѡ��ֵ��

- `*/n`����0��ʼ��ÿ��nѡһ��
- `a/n`����a��ʼ��ÿ��nѡһ��
- `a-b/n`����a��b��Χ�ڣ�ÿ��nѡһ��

```
*/2 * * * * *      // ÿ2�루0,2,4,6...�룩
5/20 * * * * *     // ÿ���ӵ�5�롢25�롢45��
0 */30 * * * *     // ÿ30����
0 0 0 */5 * *      // ÿ5��
```

#### 6. �ڼ������ڼ� `d#k` �� `d#Lk`

�������ֶ�֧�֣����ڱ���"ÿ�µڼ������ڼ�"��

- `d#k`��ÿ�µ�k������d
- `d#Lk`��ÿ�µ�����k������d

```
0 0 0 ? ? 1#1      // ÿ�µ�1����һ�賿
0 0 0 ? ? 5#2      // ÿ�µ�2�������賿
0 0 0 ? ? 1#L1     // ÿ�����1����һ�賿
0 0 0 ? ? 3-5#L2   // ÿ�µ�����2�������������賿
```

**ע��**��`#` �﷨�Ὣ������ģ 7����˿����� 7 ��ʾ���ա�

---

## ����ƫ��

### Ĭ����Ϊ

Cron Ĭ�ϲ��� Linux/.NET ���
- `0` ��ʾ���� (Sunday)
- `1` ��ʾ��һ (Monday)
- `2` ��ʾ�ܶ� (Tuesday)
- ...
- `6` ��ʾ���� (Saturday)

### Sunday ����

`Sunday` �������ڵ���"����"��Ӧ������ƫ�ƣ�

```csharp
var cron = new Cron();
cron.Sunday = 0;  // Ĭ��ֵ��0��ʾ����
// 0=���գ�1=��һ��2=�ܶ�...

cron.Sunday = 1;  // �޸�Ϊ1��ʾ����
// 1=���գ�2=��һ��3=�ܶ�...
```

**ʹ�ý���**��
- һ������±���Ĭ�� `Sunday = 0` ����
- �����Ҫ��������ϵͳ����ĳЩ���ݿ⣩���ɵ���Ϊ `Sunday = 1`
- `Parse` ���������Զ��ƶ� `Sunday`����Ҫ�ֶ�����

---

## ���� API

### IsTime - �ж�ʱ���Ƿ�ƥ��

```csharp
/// <summary>ָ��ʱ���Ƿ�λ�ڱ���ʽ֮��</summary>
/// <param name="time">Ҫ�жϵ�ʱ��</param>
/// <returns>�Ƿ�ƥ��</returns>
public Boolean IsTime(DateTime time)
```

**ʾ��**��
```csharp
var cron = new Cron("0 0 2 * * 1-5");
var time = new DateTime(2025, 1, 6, 2, 0, 0);
if (cron.IsTime(time))
{
    Console.WriteLine("ƥ��");
}
```

**ע������**��
- �ж�ʱ�ῼ���롢�֡�ʱ���ա��¡���������ά��
- �����ֶ�֧��"�ڼ������ڼ�"�ĸ����ж�
- ʱ��ᰴ�� `Sunday` ���Խ������ڼ���

### GetNext - ��ȡ��һ��ִ��ʱ��

```csharp
/// <summary>���ָ��ʱ��֮�����һ��ִ��ʱ�䣬����ָ��ʱ��</summary>
/// <param name="time">�Ӹ�ʱ�������һ���������һ��ִ��ʱ��</param>
/// <returns>��һ��ִ��ʱ�䣨�뼶�������û��ƥ���򷵻���Сʱ��</returns>
public DateTime GetNext(DateTime time)
```

**ʾ��**��
```csharp
var cron = new Cron("0 0 2 * * *");  // ÿ���賿2��
var now = DateTime.Now;
var next = cron.GetNext(now);
Console.WriteLine($"��һ��ִ�У�{next:yyyy-MM-dd HH:mm:ss}");
```

**ע������**��
- �������ʱ����к��루�� 09:14:23.456��������ǰ���뵽��һ�루09:14:24�����ټ���
- ���ص�ʱ�䲻���������ʱ�䱾��
- ���1�����Ҳ���ƥ��ʱ�䣬���� `DateTime.MinValue`
- **���ܾ���**���÷���ͨ������������ң�������1�꣬���ʺ�Ƶ������

### GetPrevious - ��ȡ��һ��ִ��ʱ��

```csharp
/// <summary>�����ָ��ʱ����ϱ���ʽ�������ȥʱ�䣨�뼶��</summary>
/// <param name="time">��׼ʱ��</param>
/// <returns>��һ��ִ��ʱ�䣬���û��ƥ���򷵻���Сʱ��</returns>
public DateTime GetPrevious(DateTime time)
```

**ʾ��**��
```csharp
var cron = new Cron("0 0 2 * * *");
var prev = cron.GetPrevious(DateTime.Now);
Console.WriteLine($"��һ��ִ�У�{prev:yyyy-MM-dd HH:mm:ss}");
```

### ���� Cron ����

```csharp
/// <summary>��һ��Cron����ʽ����ȡ��һ��ִ��ʱ��</summary>
public static DateTime GetNext(String[] crons, DateTime time)

/// <summary>��һ��Cron����ʽ����ȡǰһ��ִ��ʱ��</summary>
public static DateTime GetPrevious(String[] crons, DateTime time)
```

**ʾ��**��
```csharp
var crons = new[] { "0 0 2 * * *", "0 0 14 * * *" };  // ÿ��2���14��
var next = Cron.GetNext(crons, DateTime.Now);
Console.WriteLine($"��һ��ִ�У�{next}");
```

---

## ��� TimerX ʹ��

Cron �����ʹ�ó�������� `TimerX` ʵ�ֶ�ʱ����

```csharp
using NewLife.Threading;

// ���� Cron ��ʱ����ÿ������������8��ִ��
var timer = new TimerX(state =>
{
    Console.WriteLine($"ִ������{DateTime.Now}");
}, null, "0 0 8 * * 1-5");

// ֧�ֶ�� Cron ����ʽ���ֺŷָ�
var timer2 = new TimerX(state =>
{
    Console.WriteLine("ִ������");
}, null, "0 0 2 * * 1-5;0 0 3 * * 6");  // ������2�㣬����3��

// ʹ����ϼǵ��ͷ�
timer.Dispose();
timer2.Dispose();
```

�����[TimerX ʹ���ֲ�](timerx-�߼���ʱ��TimerX.md)

---

## ���ñ���ʽʾ��

### ÿ��/ÿ��/ÿʱ

```
* * * * * *        // ÿ��
*/2 * * * * *      // ÿ2��
*/5 * * * * *      // ÿ5��
0 * * * * *        // ÿ����
0 */5 * * * *      // ÿ5����
0 */15 * * * *     // ÿ15����
0 */30 * * * *     // ÿ30����
0 0 * * * *        // ÿСʱ
0 0 */2 * * *      // ÿ2Сʱ
```

### ÿ��̶�ʱ��

```
0 0 0 * * *        // ÿ���賿0��
0 0 1 * * *        // ÿ���賿1��
0 30 8 * * *       // ÿ��8��30��
0 0 12 * * *       // ÿ������12��
0 0 23 * * *       // ÿ������23��
0 0 0,12 * * *     // ÿ��0���12��
0 0 8,12,18 * * *  // ÿ��8�㡢12�㡢18��
```

### ������/��ĩ

```
0 0 9 * * 1-5      // ÿ������������9��
0 0 2 * * 1-5      // ÿ���������賿2��
0 0 10 * * 6,0     // ÿ����ĩ�����������գ�10��
0 0 0 * * 1        // ÿ��һ�賿
0 0 0 * * 5        // ÿ�����賿
```

### ÿ�¹̶�����

```
0 0 0 1 * *        // ÿ��1���賿
0 0 0 15 * *       // ÿ��15���賿
0 0 0 1,15 * *     // ÿ��1�ź�15���賿
0 0 0 L * *        // ÿ�����һ���賿����������⴦����
```

### ���ӳ���

```
0 0 2 * * 1-5      // ÿ���������賿2��
5/20 * * * * *     // ÿ���ӵ�5�롢25�롢45��
0 0 0 ? ? 1#1      // ÿ�µ�1����һ�賿
0 0 0 ? ? 5#L1     // ÿ�����1�������賿
0 0 0 1-7 * 1      // ÿ�µ�һ����һ�賿����һ��д����
```

---

## ע������������

### ���ܿ���

`GetNext` �� `GetPrevious` ����ͨ��**�������**ʵ�֣������ص㣺
- **�ʺϳ���**����Ƶ�ʵ��ã��綨ʱ�����ʼ��ʱ������һ��ִ��ʱ��
- **���ʺϳ���**����Ƶ���á�ʵʱ·�����ȵ����
- **��������**��������1�꣨Լ3ǧ���ѭ����

**����**��
- �� `TimerX` ��ʼ��ʱ����һ����һ��ִ��ʱ�伴��
- ������ѭ����Ƶ������
- ��������ܳ�������������ʵ���㷨

### ��֧������ֶ�

��ǰʵ�ֽ�֧��6���ֶΣ��롢�֡�ʱ���ա��¡����ڣ���**��֧�ֵ�7���ֶ�"��"**��

### �ֶ�ȱʡ����

- �������ʽ����6���ֶΣ�ȱʡ�ֶ�Ĭ��Ϊ `*`
- ���磺`0 0 2 * *` �ᱻ����Ϊ `0 0 2 * * *`

### �����ֶ�������

- ���ڼ����� `Sunday` ����Ӱ��
- `#` �﷨�Ὣ������ģ7����� 7 Ҳ���Ա�ʾ����
- ���ڷ�Χ�� 0-6������� `Sunday` ����Ϊ 1-7��

---

## Դ�����

### ��������

```csharp
public Boolean Parse(String expression)
{
    var ss = expression.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    
    // ������
    if (!TryParse(ss[0], 0, 60, out var vs)) return false;
    Seconds = vs;
    
    // ������
    if (!TryParse(ss.Length > 1 ? ss[1] : "*", 0, 60, out vs)) return false;
    Minutes = vs;
    
    // ... ���ν���ʱ���ա���
    
    // �������ڣ����⴦����
    var weeks = new Dictionary<Int32, Int32>();
    if (!TryParseWeek(ss.Length > 5 ? ss[5] : "*", 0, 7, weeks)) return false;
    DaysOfWeek = weeks;
    
    return true;
}
```

### ƥ���ж�

```csharp
public Boolean IsTime(DateTime time)
{
    // ����ʱ���ж�
    if (!Seconds.Contains(time.Second) ||
        !Minutes.Contains(time.Minute) ||
        !Hours.Contains(time.Hour) ||
        !DaysOfMonth.Contains(time.Day) ||
        !Months.Contains(time.Month))
        return false;
    
    // �����жϣ����� Sunday ƫ�ƣ�
    var w = (Int32)time.DayOfWeek + Sunday;
    if (!DaysOfWeek.TryGetValue(w, out var index)) return false;
    
    // �ڼ������ڼ��жϣ�index > 0 ��ʾ�����ڼ�����< 0 ��ʾ������
    // ... �����߼�
    
    return true;
}
```

---

## ��������

### 1. ����ʽ����ʧ�ܣ�

����﷨�Ƿ���ȷ��
- �ֶ������Ƿ�Ϊ6��������٣�ȱʡĬ��`*`��
- ��Χֵ�Ƿ�����Ч��Χ��
- ����ֵ�﷨�Ƿ���ȷ

```csharp
var cron = new Cron();
if (!cron.Parse("0 0 2 * * 1-5"))
{
    Console.WriteLine("����ʧ��");
}
```

### 2. GetNext ���� MinValue��

˵��1�����Ҳ���ƥ���ʱ�䣬����ԭ��
- ����ʽ����ì�ܣ��� `0 0 0 31 2 *` 2��û��31�ţ�
- ���ں����ڳ�ͻ

### 3. ���ڼ��㲻�ԣ�

��� `Sunday` �������ã�
```csharp
var cron = new Cron("0 0 0 * * 1");
cron.Sunday = 0;  // ȷ��ƫ����
```

### 4. ����Ӱ����㣿

`GetNext` ���Զ��������룬���϶��뵽��һ�룺
```csharp
var time = new DateTime(2025, 1, 1, 9, 14, 23, 456);  // ������
var next = cron.GetNext(time);  // �� 09:14:24 ��ʼ����
```

---

## �ο�����

- **TimerX �ĵ�**: [timerx-�߼���ʱ��TimerX.md](timerx-�߼���ʱ��TimerX.md)
- **������Cron�ο�**: https://help.aliyun.com/document_detail/64769.html
- 历史文档已归档，当前请以仓库内 Doc 为准
- **Դ��**: https://github.com/PeiKeSmart/DH.NCore/blob/master/DH.NCore/Threading/Cron.cs

---

## ������־

- **2025-01**: �����ĵ���������ϸʾ����Դ�����
- **2024**: ֧�� .NET 9.0
- **2023**: �Ż���������
- **2022**: ��������Cron���㷽��
- **2020**: ��ʼ�汾��֧�ֻ���Cron�﷨
