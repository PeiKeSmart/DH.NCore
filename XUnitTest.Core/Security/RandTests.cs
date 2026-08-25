using NewLife.Data;
using NewLife.Security;
using Xunit;

namespace XUnitTest.Security
{
    public class RandTests
    {
        [Fact]
        public void Fill()
        {
            var area = new GeoArea();
            Rand.Fill(area);

            Assert.True(area.Code > 0);
            Assert.NotEmpty(area.Name);
        }

        [Fact(DisplayName = "NextBytes返回指定长度非空数组")]
        public void NextBytes()
        {
            var buf = Rand.NextBytes(128);

            Assert.NotNull(buf);
            Assert.Equal(128, buf.Length);
            // 加密安全随机几乎不可能全零
            Assert.Contains(buf, b => b != 0);
        }
    }
}
