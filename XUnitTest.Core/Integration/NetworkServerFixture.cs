using System.Net.Sockets;
using System.Text;

using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Model;
using NewLife.Net;

using Xunit;

namespace XUnitTest.Integration;

/// <summary>NetworkServer 集成测试固定装置，复现 Samples/Zero.Server 的逻辑</summary>
public class NetworkServerFixture : IDisposable
{
    /// <summary>网络服务端实例</summary>
    public NetServer Server { get; }

    public NetworkServerFixture()
    {
        XTrace.UseConsole();

        var server = new MyNetServer
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

/// <summary>定义服务端，用于管理所有网络会话，对应 Samples/Zero.Server/MyNetServer.cs</summary>
class MyNetServer : NetServer<MyNetSession>
{
}

/// <summary>定义会话。连接时发欢迎语，收到数据后返回反转字符串</summary>
class MyNetSession : NetSession<MyNetServer>
{
    /// <summary>客户端连接</summary>
    protected override void OnConnected()
    {
        // 发送欢迎语
        Send($"Welcome to visit {Environment.MachineName}!  [{Remote}]\r\n");

        base.OnConnected();
    }

    /// <summary>客户端断开连接</summary>
    /// <param name="reason">断开原因</param>
    protected override void OnDisconnected(String reason)
    {
        WriteLog("客户端 {0} 已断开连接。{1}", Remote, reason);

        base.OnDisconnected(reason);
    }

    /// <summary>收到客户端数据</summary>
    /// <param name="e">接收事件参数</param>
    protected override void OnReceive(ReceivedEventArgs e)
    {
        var packet = e.Packet;
        if (packet == null || packet.Length == 0) return;

        WriteLog("收到：{0}", packet.ToStr());

        // 把收到的字符串反转后发回
        Send(packet.ToStr().Reverse().Join(null));
    }

    /// <summary>出错</summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">异常事件参数</param>
    protected override void OnError(Object sender, ExceptionEventArgs e)
    {
        WriteLog("[{0}] 错误：{1}", e.Action, e.Exception?.GetTrue().Message);

        base.OnError(sender, e);
    }
}

/// <summary>NetworkServer 集成测试，验证 TCP/UDP 连接与收发功能</summary>
[TestCaseOrderer("NewLife.UnitTest.DefaultOrderer", "NewLife.UnitTest")]
public class NetworkServerIntegrationTests : IClassFixture<NetworkServerFixture>
{
    private readonly NetworkServerFixture _fixture;

    public NetworkServerIntegrationTests(NetworkServerFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = "01-服务端已启动且端口已分配")]
    public void Test01_ServerStarted()
    {
        Assert.True(_fixture.Server.Active, "服务端应处于运行状态");
        Assert.True(_fixture.Server.Port > 0, "端口应已分配");

        XTrace.WriteLine("NetworkServer 已在端口 {0} 上启动", _fixture.Server.Port);
    }

    [Fact(DisplayName = "02-TcpClient 连接后收到欢迎消息")]
    public async Task Test02_TcpClientWelcome()
    {
        var port = _fixture.Server.Port;
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", port);
        var ns = client.GetStream();

        // 服务端连接后主动发欢迎语
        var buf = new Byte[1024];
        ns.ReadTimeout = 5_000;
        var count = await ns.ReadAsync(buf);
        var welcome = Encoding.UTF8.GetString(buf, 0, count);

        Assert.Contains("Welcome", welcome);
        XTrace.WriteLine("<= {0}", welcome.Trim());
    }

    [Fact(DisplayName = "03-TcpClient 发送消息后收到反转字符串")]
    public async Task Test03_TcpClientEcho()
    {
        var port = _fixture.Server.Port;
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", port);
        var ns = client.GetStream();

        // 先收欢迎语
        var buf = new Byte[1024];
        ns.ReadTimeout = 5_000;
        await ns.ReadAsync(buf);

        // 发送数据
        const String msg = "Hello NewLife";
        var msgBytes = Encoding.UTF8.GetBytes(msg);
        await ns.WriteAsync(msgBytes);

        // 接收反转后的数据
        var count = await ns.ReadAsync(buf);
        var reply = Encoding.UTF8.GetString(buf, 0, count);

        Assert.Equal("efiLweN olleH", reply);
        XTrace.WriteLine("<= {0}", reply);
    }

    [Fact(DisplayName = "04-UdpClient 发包后收到欢迎消息和反转回显")]
    public async Task Test04_UdpClientEcho()
    {
        var port = _fixture.Server.Port;
        using var udp = new UdpClient();

        var endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port);
        const String msg = "Hello NewLife";
        var msgBytes = Encoding.UTF8.GetBytes(msg);

        udp.Client.ReceiveTimeout = 5_000;

        // 发送第一个包，服务端才建立 UDP 会话
        await udp.SendAsync(msgBytes, msgBytes.Length, endpoint);

        // 收到 welcome（OnConnected 触发）
        var result1 = await udp.ReceiveAsync();
        var welcome = Encoding.UTF8.GetString(result1.Buffer);
        Assert.Contains("Welcome", welcome);
        XTrace.WriteLine("<= {0}", welcome.Trim());

        // 收到反转字符串（OnReceive 触发）
        var result2 = await udp.ReceiveAsync();
        var reply = Encoding.UTF8.GetString(result2.Buffer);
        Assert.Equal("efiLweN olleH", reply);
        XTrace.WriteLine("<= {0}", reply);

        // 发空包通知服务端关闭 UDP 会话
        await udp.SendAsync([], 0, endpoint);
    }

    [Fact(DisplayName = "05-ISocketClient(TCP) 完整收发流程")]
    public async Task Test05_TcpSession()
    {
        var port = _fixture.Server.Port;
        var uri = new NetUri($"tcp://127.0.0.1:{port}");
        var client = uri.CreateRemote();
        client.Name = "集成测试Tcp客户";
        client.Log = XTrace.Log;

        // 关闭异步模式，使用同步 ReceiveAsync 以便确定包边界
        if (client is TcpSession tcp) tcp.MaxAsync = 0;

        // 接收欢迎语（内部自动建立连接）
        using var welcome = await client.ReceiveAsync(default).WaitAsync(TimeSpan.FromSeconds(5));
        var welcomeStr = welcome.ToStr();
        Assert.Contains("Welcome", welcomeStr);
        XTrace.WriteLine("<= {0}", welcomeStr.Trim());

        // 发送数据
        const String msg = "Hello NewLife";
        client.Send(msg);
        XTrace.WriteLine("=> {0}", msg);

        // 接收反转字符串
        using var reply = await client.ReceiveAsync(default).WaitAsync(TimeSpan.FromSeconds(5));
        var replyStr = reply.ToStr();
        Assert.Equal("efiLweN olleH", replyStr);
        XTrace.WriteLine("<= {0}", replyStr);

        client.Close("Test05Done");
    }
}
