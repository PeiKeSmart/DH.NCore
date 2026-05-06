using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;

using Xunit;

namespace XUnitTest.Integration;

/// <summary>EchoServer 集成测试固定装置，复现 Samples/Zero.EchoServer 的逻辑</summary>
public class EchoServerFixture : IDisposable
{
    /// <summary>回声服务端实例</summary>
    public NetServer Server { get; }

    public EchoServerFixture()
    {
        XTrace.UseConsole();

        var server = new EchoNetServer
        {
            Port = 0,
            Log = XTrace.Log,
#if DEBUG
            SessionLog = XTrace.Log,
#endif
        };
        Server = server;
        Server.Start();
    }

    public void Dispose() => Server?.Stop("IntegrationTestDone");
}

/// <summary>定义回声服务端，把收到的数据原样发回去，用于网络性能压测</summary>
class EchoNetServer : NetServer<EchoSession>
{
}

/// <summary>回声会话，收到数据后原样返回</summary>
class EchoSession : NetSession<EchoNetServer>
{
    /// <summary>收到客户端数据</summary>
    /// <param name="e"></param>
    protected override void OnReceive(ReceivedEventArgs e)
    {
        var packet = e.Packet;
        if (packet == null || packet.Length == 0) return;

        // 把收到的数据发回去
        Send(packet);
    }
}

/// <summary>EchoServer 集成测试，验证 TCP/UDP 回显功能</summary>
[TestCaseOrderer("NewLife.UnitTest.DefaultOrderer", "NewLife.UnitTest")]
public class EchoServerIntegrationTests : IClassFixture<EchoServerFixture>
{
    private readonly EchoServerFixture _fixture;

    public EchoServerIntegrationTests(EchoServerFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = "01-服务端已启动且端口已分配")]
    public void Test01_ServerStarted()
    {
        Assert.True(_fixture.Server.Active, "服务端应处于运行状态");
        Assert.True(_fixture.Server.Port > 0, "端口应已分配");

        XTrace.WriteLine("EchoServer 已在端口 {0} 上启动", _fixture.Server.Port);
    }

    [Fact(DisplayName = "02-TCP 回显 16 字节小包")]
    public async Task Test02_TcpEcho16Bytes()
    {
        var port = _fixture.Server.Port;
        var client = new NetUri($"tcp://127.0.0.1:{port}").CreateRemote();

        var payload = new Byte[16];
        Random.Shared.NextBytes(payload);

        var wait = new TaskCompletionSource<Byte[]>();
        client.Received += (s, e) => wait.TrySetResult(e.GetBytes());

        client.Open();
        client.Send(payload);

        var received = await wait.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(payload, received);

        client.Close("Test02Done");
    }

    [Fact(DisplayName = "03-TCP 回显 1024 字节大包")]
    public async Task Test03_TcpEcho1024Bytes()
    {
        var port = _fixture.Server.Port;
        var client = new NetUri($"tcp://127.0.0.1:{port}").CreateRemote();

        var payload = new Byte[1024];
        Random.Shared.NextBytes(payload);

        var wait = new TaskCompletionSource<Byte[]>();
        client.Received += (s, e) => wait.TrySetResult(e.GetBytes());

        client.Open();
        client.Send(payload);

        var received = await wait.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(payload, received);

        client.Close("Test03Done");
    }

    [Fact(DisplayName = "04-UDP 回显消息")]
    public async Task Test04_UdpEcho()
    {
        var port = _fixture.Server.Port;
        var client = new NetUri($"udp://127.0.0.1:{port}").CreateRemote();

        var payload = new Byte[32];
        Random.Shared.NextBytes(payload);

        var wait = new TaskCompletionSource<Byte[]>();
        client.Received += (s, e) => wait.TrySetResult(e.GetBytes());

        client.Open();
        client.Send(payload);

        var received = await wait.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(payload, received);

        client.Close("Test04Done");
    }
}
