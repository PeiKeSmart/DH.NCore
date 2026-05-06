using System.Net.WebSockets;
using System.Text;

using NewLife;
using NewLife.Data;
using NewLife.Http;
using NewLife.Log;
using NewLife.Remoting;

using Xunit;

namespace XUnitTest.Integration;

/// <summary>HttpServer 集成测试固定装置，复现 Samples/Zero.HttpServer 的路由配置</summary>
public class HttpServerFixture : IDisposable
{
    /// <summary>HTTP 服务端实例</summary>
    public HttpServer Server { get; }

    /// <summary>服务端基础地址</summary>
    public Uri BaseUri { get; }

    public HttpServerFixture()
    {
        XTrace.UseConsole();

        var server = new HttpServer
        {
            Name = "集成测试Http服务器",
            Port = 0,
            Log = XTrace.Log,
#if DEBUG
            SessionLog = XTrace.Log,
#endif
        };

        // 简单路径，返回字符串
        server.Map("/", () => "<h1>Hello NewLife!</h1></br> " + DateTime.Now.ToFullString());
        server.Map("/user", (String act, Int32 uid) => new { code = 0, data = $"User.{act}({uid}) success!" });

        // 自定义处理器，操作 Http 上下文
        server.Map("/my", new IntegrationHttpHandler());

        // 自定义控制器
        server.MapController<ApiController>("/api");

        // WebSocket 处理器
        server.Map("/ws", new WebSocketHandler());

        server.Start();

        Server = server;
        BaseUri = new Uri($"http://127.0.0.1:{server.Port}");
    }

    public void Dispose() => Server?.Dispose();
}

/// <summary>内联 HttpHandler，对应 Samples/Zero.HttpServer/MyHttpHandler.cs 的核心逻辑</summary>
class IntegrationHttpHandler : IHttpHandler
{
    /// <summary>处理请求</summary>
    /// <param name="context">Http 上下文</param>
    public void ProcessRequest(IHttpContext context)
    {
        var name = context.Parameters["name"];
        var html = $"<h2>你好，<span color=\"red\">{name}</span></h2>";
        context.Response.SetResult(html);
    }
}

/// <summary>HttpServer 集成测试，验证 HTTP/WebSocket 功能</summary>
[TestCaseOrderer("NewLife.UnitTest.DefaultOrderer", "NewLife.UnitTest")]
public class HttpServerIntegrationTests : IClassFixture<HttpServerFixture>
{
    private readonly HttpServerFixture _fixture;

    public HttpServerIntegrationTests(HttpServerFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = "01-服务端已启动且端口已分配")]
    public void Test01_ServerStarted()
    {
        Assert.True(_fixture.Server.Active, "服务端应处于运行状态");
        Assert.True(_fixture.Server.Port > 0, "端口应已分配");

        XTrace.WriteLine("HttpServer 已在端口 {0} 上启动", _fixture.Server.Port);
    }

    [Fact(DisplayName = "02-GET / 返回 Hello NewLife")]
    public async Task Test02_HttpGetRoot()
    {
        using var client = new HttpClient { BaseAddress = _fixture.BaseUri };
        var html = await client.GetStringAsync("/");

        Assert.NotEmpty(html);
        Assert.Contains("Hello NewLife", html);

        XTrace.WriteLine("GET / 响应：{0}", html);
    }

    [Fact(DisplayName = "03-GET /user 返回正确的 API 结果")]
    public async Task Test03_UserApiRequest()
    {
        var http = new ApiHttpClient(_fixture.BaseUri.ToString())
        {
            Log = XTrace.Log,
        };

        var rs = await http.GetAsync<String>("/user", new { act = "Delete", uid = 1234 });

        Assert.Equal("User.Delete(1234) success!", rs);

        XTrace.WriteLine("GET /user 响应：{0}", rs);
    }

    [Fact(DisplayName = "04-GET /my 自定义处理器返回正确响应")]
    public async Task Test04_CustomHandler()
    {
        using var client = new HttpClient { BaseAddress = _fixture.BaseUri };
        var html = await client.GetStringAsync("/my?name=stone");

        Assert.Equal("<h2>你好，<span color=\"red\">stone</span></h2>", html);

        XTrace.WriteLine("GET /my 响应：{0}", html);
    }

    [Fact(DisplayName = "05-GET /api/info 控制器返回机器信息")]
    public async Task Test05_ApiInfoRequest()
    {
        var http = new ApiHttpClient(_fixture.BaseUri.ToString())
        {
            Log = XTrace.Log,
        };

        var rs = await http.GetAsync<Object>("/api/info", new { state = "test" });

        Assert.NotNull(rs);

        var json = NewLife.Serialization.JsonHelper.ToJson(rs);
        Assert.Contains("MachineName", json);

        XTrace.WriteLine("GET /api/info 响应：{0}", json);
    }

    [Fact(DisplayName = "06-WebSocket 连接收发消息")]
    public async Task Test06_WebSocketConnect()
    {
        var wsUri = new Uri($"ws://127.0.0.1:{_fixture.Server.Port}/ws");
        using var ws = new ClientWebSocket();

        await ws.ConnectAsync(wsUri, default);
        Assert.Equal(WebSocketState.Open, ws.State);

        var msg = "Hello NewLife";
        await ws.SendAsync(Encoding.UTF8.GetBytes(msg), System.Net.WebSockets.WebSocketMessageType.Text, true, default);

        var buf = new Byte[1024];
        var result = await ws.ReceiveAsync(buf, default);
        var reply = Encoding.UTF8.GetString(buf, 0, result.Count);

        // WebSocketHandler.SendAll 会把消息广播回来，格式：[remote]说，msg
        Assert.Contains(msg, reply);
        XTrace.WriteLine("WebSocket 收到：{0}", reply);

        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "测试完成", default);
    }
}
