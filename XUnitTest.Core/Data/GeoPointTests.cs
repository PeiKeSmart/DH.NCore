using System.ComponentModel;
using System.Text.Json;
using NewLife.Data;
using Xunit;

namespace XUnitTest.Data;

public class GeoPointTests
{
    [Fact]
    [DisplayName("默认值_为零坐标")]
    public void Default_IsZero()
    {
        var p = default(GeoPoint);
        Assert.Equal(0, p.Longitude);
        Assert.Equal(0, p.Latitude);

        // 无参构造与默认值等价
        var p2 = new GeoPoint();
        Assert.Equal(p, p2);
    }

    [Fact]
    [DisplayName("构造_经纬度_赋值正确")]
    public void Ctor_Valid_AssignsValues()
    {
        var p = new GeoPoint(116.402843, 39.999375);
        Assert.Equal(116.402843, p.Longitude);
        Assert.Equal(39.999375, p.Latitude);
    }

    [Fact]
    [DisplayName("值语义_等值与哈希一致")]
    public void ValueEquality_Works()
    {
        var p1 = new GeoPoint(116.4, 39.9);
        var p2 = new GeoPoint(116.4, 39.9);
        var p3 = new GeoPoint(116.4, 39.8);

        Assert.Equal(p1, p2);
        Assert.True(p1 == p2);
        Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
        Assert.NotEqual(p1, p3);
    }

    [Theory]
    [DisplayName("ToString_输出经度纬度逗号分隔")]
    [InlineData(116.4, 39.9, "116.4,39.9")]
    [InlineData(0, 0, "0,0")]
    public void ToString_FormatsAsLngLat(Double lng, Double lat, String expected)
    {
        var p = new GeoPoint(lng, lat);
        Assert.Equal(expected, p.ToString());
    }

    [Theory]
    [DisplayName("Parse_有效字符串_解析成功")]
    [InlineData("116.4,39.9", 116.4, 39.9)]
    [InlineData("116.402843,39.999375", 116.402843, 39.999375)]
    [InlineData("116.4,39.9,extra", 116.4, 39.9)] // 多余段忽略，兼容旧行为
    public void Parse_Valid_ReturnsPoint(String input, Double lng, Double lat)
    {
        var p = GeoPoint.Parse(input);
        Assert.Equal(lng, p.Longitude);
        Assert.Equal(lat, p.Latitude);
    }

    [Theory]
    [DisplayName("Parse_非法字符串_抛出异常")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("abc")]
    [InlineData("abc,def")]
    [InlineData("116.4")]
    public void Parse_Invalid_Throws(String? input)
    {
        Assert.Throws<FormatException>(() => GeoPoint.Parse(input));
    }

    [Theory]
    [DisplayName("TryParse_各种输入_返回是否成功")]
    [InlineData("116.4,39.9", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("abc,def", false)]
    [InlineData("116.4", false)]
    public void TryParse_ReturnsBool(String? input, Boolean expected)
    {
        var ok = GeoPoint.TryParse(input, out var p);
        Assert.Equal(expected, ok);
        if (!ok) Assert.Equal(default, p);
    }

    [Fact]
    [DisplayName("ToString_Parse_往返一致")]
    public void ToString_Parse_RoundTrip()
    {
        var p = new GeoPoint(116.402843, 39.999375);
        var p2 = GeoPoint.Parse(p.ToString());
        Assert.Equal(p, p2);
    }

    [Fact]
    [DisplayName("Encode_默认9位_与GeoHash一致")]
    public void Encode_Default_MatchesGeoHash()
    {
        // 与 GeoHashTests 的已知值互验（鸟巢）
        var p = new GeoPoint(116.402843, 39.999375);
        Assert.Equal("wx4g8c9vn", p.Encode());
        Assert.Equal(GeoHash.Encode(116.402843, 39.999375), p.Encode());
    }

    [Fact]
    [DisplayName("Encode_指定位数_生效")]
    public void Encode_WithLevel_Works()
    {
        var p = new GeoPoint(116.402843, 39.999375);
        Assert.Equal(6, p.Encode(6).Length);
        // 具名参数兼容旧 API
        Assert.Equal("wx4g8c9vn", p.Encode(level: 9));
    }

    [Fact]
    [DisplayName("Decode_GeoHash字符串_还原坐标点")]
    public void Decode_ReturnsPoint()
    {
        var p = GeoPoint.Decode("wx4g8c9vn");
        Assert.True(Math.Abs(116.402843 - p.Longitude) < 0.0001);
        Assert.True(Math.Abs(39.999375 - p.Latitude) < 0.0001);
    }

    [Fact]
    [DisplayName("Decode_非法字符_抛出异常")]
    public void Decode_Invalid_Throws()
    {
        // 包含非法字符 O
        Assert.Throws<ArgumentException>(() => GeoPoint.Decode("wx4g8O9vn"));
    }

    [Fact]
    [DisplayName("TryDecode_非法字符串_返回false")]
    public void TryDecode_Invalid_ReturnsFalse()
    {
        Assert.False(GeoPoint.TryDecode("wx4g8O9vn", out var p));
        Assert.Equal(default, p);

        Assert.True(GeoPoint.TryDecode("wx4g8c9vn", out var p2));
        Assert.True(Math.Abs(116.402843 - p2.Longitude) < 0.0001);
    }

    [Fact]
    [DisplayName("Encode_Decode_往返一致")]
    public void Encode_Decode_RoundTrip()
    {
        var p = new GeoPoint(116.402843, 39.999375);
        var hash = p.Encode();
        var p2 = GeoPoint.Decode(hash);
        // 9位精度约2米，往返误差极小
        Assert.True(Math.Abs(p.Longitude - p2.Longitude) < 0.0001);
        Assert.True(Math.Abs(p.Latitude - p2.Latitude) < 0.0001);
    }

    #region 值语义
    [Fact]
    [DisplayName("Deconstruct_解构为经纬度元组")]
    public void Deconstruct_ReturnsTuple()
    {
        var p = new GeoPoint(116.4, 39.9);
        var (lng, lat) = p;
        Assert.Equal(116.4, lng);
        Assert.Equal(39.9, lat);
    }

    [Fact]
    [DisplayName("With_表达式_仅修改指定字段")]
    public void With_Expression_ModifiesField()
    {
        var p = new GeoPoint(116.4, 39.9);
        var p2 = p with { Latitude = 40 };
        Assert.Equal(116.4, p2.Longitude);
        Assert.Equal(40, p2.Latitude);
        // 原实例不变
        Assert.Equal(39.9, p.Latitude);
    }

    [Fact]
    [DisplayName("不等于_运算符_不同值返回true")]
    public void NotEquals_Works()
    {
        var p1 = new GeoPoint(116.4, 39.9);
        var p2 = new GeoPoint(116.4, 39.8);
        Assert.True(p1 != p2);
    }

    [Fact]
    [DisplayName("Equals_装箱比较_一致")]
    public void Equals_Boxed_Works()
    {
        var p1 = new GeoPoint(116.4, 39.9);
        Object o = new GeoPoint(116.4, 39.9);
        Assert.True(p1.Equals(o));
        Assert.False(p1.Equals(new GeoPoint(1, 2)));
        // 与不同类型不相等
        Assert.False(p1.Equals("116.4,39.9"));
    }

    [Fact]
    [DisplayName("NaN_值语义_相等与哈希一致")]
    public void NaN_ValueSemantics()
    {
        var p1 = new GeoPoint(Double.NaN, 39.9);
        var p2 = new GeoPoint(Double.NaN, 39.9);
        Assert.Equal(p1, p2);
        Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
    }
    #endregion

    #region ToString
    [Theory]
    [DisplayName("ToString_负数与高精度_格式正确")]
    [InlineData(-73.9857, 40.7484, "-73.9857,40.7484")]
    [InlineData(116.402843, 39.999375, "116.402843,39.999375")]
    public void ToString_EdgeValues_FormatCorrect(Double lng, Double lat, String expected)
    {
        var p = new GeoPoint(lng, lat);
        Assert.Equal(expected, p.ToString());
    }
    #endregion

    #region Parse
    [Theory]
    [DisplayName("Parse_带空白_解析成功")]
    [InlineData(" 116.4 , 39.9 ", 116.4, 39.9)]
    [InlineData("\t116.4,39.9", 116.4, 39.9)]
    public void Parse_WithWhitespace_Works(String input, Double lng, Double lat)
    {
        var p = GeoPoint.Parse(input);
        Assert.Equal(lng, p.Longitude);
        Assert.Equal(lat, p.Latitude);
    }

    [Theory]
    [DisplayName("Parse_负坐标_解析成功")]
    [InlineData("-73.9857,40.7484", -73.9857, 40.7484)]
    [InlineData("-33.8688,-151.2093", -33.8688, -151.2093)]
    public void Parse_Negative_Works(String input, Double lng, Double lat)
    {
        var p = GeoPoint.Parse(input);
        Assert.Equal(lng, p.Longitude);
        Assert.Equal(lat, p.Latitude);
    }

    [Fact]
    [DisplayName("Parse_科学计数法_解析成功")]
    public void Parse_Scientific_Works()
    {
        var p = GeoPoint.Parse("1.164028e2,3.9999375e1");
        Assert.True(Math.Abs(116.4028 - p.Longitude) < 1e-4);
        Assert.True(Math.Abs(39.999375 - p.Latitude) < 1e-4);
    }
    #endregion

    #region GeoHash 编码解码边界
    [Theory]
    [DisplayName("Encode_位数裁剪_与GeoHash一致")]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(20)]
    [InlineData(12)]
    [InlineData(1)]
    public void Encode_LevelClamp_MatchesGeoHash(Int32 level)
    {
        var p = new GeoPoint(116.402843, 39.999375);
        Assert.Equal(GeoHash.Encode(116.402843, 39.999375, level), p.Encode(level));
        // 有效范围内长度等于位数（1~12 裁剪）
        Assert.Equal(Math.Clamp(level, 1, 12), p.Encode(level).Length);
    }

    [Fact]
    [DisplayName("Encode_零坐标_与GeoHash一致")]
    public void Encode_Zero_MatchesGeoHash()
    {
        var p = default(GeoPoint);
        Assert.Equal(GeoHash.Encode(0, 0), p.Encode());
        Assert.Equal(9, p.Encode().Length);
    }

    [Fact]
    [DisplayName("Decode_空字符串或null_抛出异常")]
    public void Decode_EmptyOrNull_Throws()
    {
        Assert.Throws<ArgumentException>(() => GeoPoint.Decode(""));
        Assert.Throws<ArgumentException>(() => GeoPoint.Decode(null!));
    }

    [Fact]
    [DisplayName("Decode_大写GeoHash_与GeoHash一致")]
    public void Decode_Uppercase_MatchesGeoHash()
    {
        var pLower = GeoPoint.Decode("wx4g8c9vn");
        var pUpper = GeoPoint.Decode("WX4G8C9VN");
        Assert.True(Math.Abs(pLower.Longitude - pUpper.Longitude) < 1e-10);
        Assert.True(Math.Abs(pLower.Latitude - pUpper.Latitude) < 1e-10);
    }

    [Fact]
    [DisplayName("Decode_短位数_返回有效中心点")]
    public void Decode_ShortHash_Works()
    {
        var p = GeoPoint.Decode("wx");
        // 短位数精度低，但应返回中心点且不抛异常
        Assert.True(p.Longitude is >= -180 and <= 180);
        Assert.True(p.Latitude is >= -90 and <= 90);
    }

    [Theory]
    [DisplayName("TryDecode_空值或非法_返回false")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("wx4g8O9vn")]
    public void TryDecode_InvalidInputs_ReturnsFalse(String? input)
    {
        Assert.False(GeoPoint.TryDecode(input!, out var p));
        Assert.Equal(default, p);
    }

    [Fact]
    [DisplayName("TryDecode_大写_成功")]
    public void TryDecode_Uppercase_Works()
    {
        Assert.True(GeoPoint.TryDecode("WX4G8C9VN", out var p));
        Assert.True(Math.Abs(116.402843 - p.Longitude) < 0.0001);
        Assert.True(Math.Abs(39.999375 - p.Latitude) < 0.0001);
    }
    #endregion

    #region 序列化
    [Fact]
    [DisplayName("JSON_序列化反序列化_往返一致")]
    public void Json_RoundTrip()
    {
        var p = new GeoPoint(116.402843, 39.999375);
        var json = JsonSerializer.Serialize(p);
        var p2 = JsonSerializer.Deserialize<GeoPoint>(json);
        Assert.Equal(p, p2);
    }
    #endregion
}
