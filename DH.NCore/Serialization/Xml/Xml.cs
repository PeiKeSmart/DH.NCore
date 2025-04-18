﻿using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using NewLife.Reflection;

namespace NewLife.Serialization;

/// <summary>Xml序列化</summary>
public class Xml : FormatterBase, IXml
{
    #region 属性
    /// <summary>深度</summary>
    public Int32 Depth { get; set; }

    /// <summary>处理器列表</summary>
    public List<IXmlHandler> Handlers { get; private set; }

    /// <summary>使用特性</summary>
    public Boolean UseAttribute { get; set; }

    /// <summary>使用注释</summary>
    public Boolean UseComment { get; set; }

    /// <summary>枚举使用字符串。false时使用数字，默认true</summary>
    public Boolean EnumString { get; set; } = true;

    /// <summary>XML写入设置</summary>
    public XmlWriterSettings? Setting { get; set; }

    /// <summary>当前名称</summary>
    public String? CurrentName { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化</summary>
    public Xml()
    {
        // 遍历所有处理器实现
        var list = new List<IXmlHandler>
        {
            new XmlGeneral { Host = this },
            new XmlList { Host = this },
            new XmlComposite { Host = this }
        };
        // 根据优先级排序
        Handlers = list.OrderBy(e => e.Priority).ToList();
    }
    #endregion

    #region 处理器
    /// <summary>添加处理器</summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Xml AddHandler(IXmlHandler handler)
    {
        if (handler != null)
        {
            handler.Host = this;
            Handlers.Add(handler);
            // 根据优先级排序
            Handlers = Handlers.OrderBy(e => e.Priority).ToList();
        }

        return this;
    }

    /// <summary>添加处理器</summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="priority"></param>
    /// <returns></returns>
    public Xml AddHandler<THandler>(Int32 priority = 0) where THandler : IXmlHandler, new()
    {
        var handler = new THandler
        {
            Host = this
        };
        if (priority != 0) handler.Priority = priority;

        return AddHandler(handler);
    }
    #endregion

    #region 写入
    /// <summary>写入一个对象</summary>
    /// <param name="value">目标对象</param>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public Boolean Write(Object? value, String? name = null, Type? type = null)
    {
        if (type == null)
        {
            if (value == null) return true;

            type = value.GetType();
        }

        var writer = GetWriter();

        // 检查接口
        if (value is IXmlSerializable xml)
        {
            xml.WriteXml(writer);
            return true;
        }

        if (name.IsNullOrEmpty())
        {
            // 优先采用类型上的XmlRoot特性
            name = type.GetCustomAttributeValue<XmlRootAttribute, String>(true);
            if (name.IsNullOrEmpty()) name = GetName(type);
        }

        name = name.Replace('<', '_');
        name = name.Replace('>', '_');
        name = name.Replace('`', '_');
        CurrentName = name;

        // 一般类型为空是顶级调用
        if (Hosts.Count == 0 && Log != null && Log.Enable) WriteLog("XmlWrite {0} {1}", name ?? type.Name, value);

        // 要先写入根
        Depth++;
        if (Depth == 1) writer.WriteStartDocument();

        var rs = WriteStart(type);
        try
        {
            if (rs /*&& value != null*/)
            {
                foreach (var item in Handlers)
                {
                    if (item.Write(value, type)) return true;
                }

                if (value != null)
                    writer.WriteValue(value);

                return true;
            }

            return false;
        }
        finally
        {
            if (rs) WriteEnd();
            if (Depth == 1)
            {
                writer.WriteEndDocument();
                writer.Flush();
            }
            Depth--;
        }
    }

    Boolean IFormatterX.Write(Object? value, Type? type) => Write(value, null, type);

    /// <summary>写入开头</summary>
    /// <param name="type"></param>
    public Boolean WriteStart(Type type)
    {
        var name = CurrentName;
        if (name.IsNullOrEmpty()) return false;

        var att = UseAttribute;
        if (!att && Member?.GetCustomAttribute<XmlAttributeAttribute>() != null) att = true;
        if (att && !type.IsValueType && !type.IsBaseType()) att = false;

        var writer = GetWriter();

        // 写入注释。写特性时忽略注释
        if (UseComment && !att)
        {
            var des = "";
            if (Member != null) des = Member.GetDisplayName() ?? Member.GetDescription();
            if (des.IsNullOrEmpty() && type != null) des = type.GetDisplayName() ?? type.GetDescription();

            if (!des.IsNullOrEmpty()) writer.WriteComment(des);
        }

        if (att)
            writer.WriteStartAttribute(name);
        else
            writer.WriteStartElement(name);

        return true;
    }

    /// <summary>写入结尾</summary>
    public void WriteEnd()
    {
        var writer = GetWriter();

        if (writer.WriteState != WriteState.Start)
        {
            if (writer.WriteState == WriteState.Attribute)
                writer.WriteEndAttribute();
            else
            {
                writer.WriteEndElement();
                //替换成WriteFullEndElement方法，写入完整的结束标记。解决读取空节点（短结束标记"/ >"）发生错误。
                //writer.WriteFullEndElement();
            }
        }
    }

    private XmlWriter? _Writer;
    /// <summary>获取Xml写入器</summary>
    /// <returns></returns>
    public XmlWriter GetWriter()
    {
        if (_Writer == null)
        {
            var set = Setting?.Clone() ?? new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
            set.Encoding = Encoding;
            if (set.OmitXmlDeclaration) set.ConformanceLevel = ConformanceLevel.Auto;

            _Writer = XmlWriter.Create(Stream, set);
        }

        return _Writer;
    }
    #endregion

    #region 读取
    /// <summary>读取指定类型对象</summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Object? Read(Type type)
    {
        var value = type.As<Array>() ? null : type.CreateInstance();
        if (!TryRead(type, ref value)) throw new Exception("Read failed!");

        return value;
    }

    /// <summary>读取指定类型对象</summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Read<T>() => (T?)Read(typeof(T));

    /// <summary>尝试读取指定类型对象</summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Boolean TryRead(Type type, ref Object? value)
    {
        var reader = GetReader();
        // 移动到第一个元素
        while (reader.NodeType != XmlNodeType.Element) { if (!reader.Read()) return false; }

        if (Hosts.Count == 0 && Log != null && Log.Enable) WriteLog("XmlRead {0} {1}", type.Name, value);

        // 要先写入根
        Depth++;

        var d = reader.Depth;
        ReadStart(type);

        try
        {
            // 如果读取器层级没有递增，说明这是空节点，需要跳过
            if (reader.Depth == d + 1)
            {
                foreach (var item in Handlers)
                {
                    if (item.TryRead(type, ref value)) return true;
                }

                value = reader.ReadContentAs(type, null);
            }
        }
        finally
        {
            ReadEnd();
            Depth--;
        }

        return true;
    }

    /// <summary>读取开始</summary>
    /// <param name="type"></param>
    public void ReadStart(Type type)
    {
        var att = UseAttribute;
        if (!att && Member?.GetCustomAttribute<XmlAttributeAttribute>() != null) _ = true;

        var reader = GetReader();
        while (reader.NodeType == XmlNodeType.Comment) reader.Skip();

        CurrentName = reader.Name;
        if (reader.HasAttributes)
            reader.MoveToFirstAttribute();
        else
            reader.ReadStartElement();

        while (reader.NodeType is XmlNodeType.Comment or XmlNodeType.Whitespace) reader.Skip();
    }

    /// <summary>读取结束</summary>
    public void ReadEnd()
    {
        var reader = GetReader();
        if (reader.NodeType == XmlNodeType.Attribute) reader.Read();
        if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();
    }

    private XmlReader? _Reader;
    /// <summary>获取Xml读取器</summary>
    /// <returns></returns>
    public XmlReader GetReader()
    {
        _Reader ??= XmlReader.Create(Stream);

        return _Reader;
    }
    #endregion

    #region 辅助方法
    private static String GetName(Type type)
    {
        if (type.HasElementType)
        {
            var elmType = type.GetElementTypeEx();
            return elmType == null ? "Array" : "ArrayOf" + GetName(elmType);
        }

        var name = type.GetName();
        name = name.Replace("<", "_");
        //name = name.Replace(">", "_");
        name = name.Replace(",", "_");
        name = name.Replace(">", "");
        return name;
    }

    /// <summary>获取字符串</summary>
    /// <returns></returns>
    public String GetString() => GetBytes().ToStr(Encoding);
    #endregion
}