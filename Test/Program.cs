﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NewLife;
using NewLife.Caching;
using NewLife.Collections;
using NewLife.Common;
using NewLife.Data;
using NewLife.Http;
using NewLife.IO;
using NewLife.Log;
using NewLife.Model;
using NewLife.Net;
using NewLife.Net.Handlers;
using NewLife.Remoting;
using NewLife.Security;
using NewLife.Serialization;
using NewLife.Threading;
using NewLife.Web;
using Stardust;
using Stardust.Models;
using XCode.Membership;

namespace Test;

public class Program
{
    private static async Task Main(String[] args)
    {
        //Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1");

        XTrace.UseConsole();

        TimerScheduler.Default.Log = XTrace.Log;

        //var star = new StarFactory(null, null, null);
        //DefaultTracer.Instance = star?.Tracer;
        //(star.Tracer as StarTracer).AttachGlobal();

#if DEBUG
        XTrace.Debug = true;
        XTrace.Log.Level = LogLevel.All;

        var set = NewLife.Setting.Current;
        set.Debug = true;
        set.LogLevel = LogLevel.All;

        //new LogEventListener(new[] {
        //    "System.Runtime",
        //    "System.Diagnostics.Eventing.FrameworkEventSource",
        //    "System.Transactions.TransactionsEventSource",
        //    "Microsoft-Windows-DotNETRuntime",
        //    //"Private.InternalDiagnostics.System.Net.Sockets",
        //    "System.Net.NameResolution",
        //    //"Private.InternalDiagnostics.System.Net.NameResolution",
        //    "System.Net.Sockets",
        //    //"Private.InternalDiagnostics.System.Net.Http",
        //    "System.Net.Http",
        //    //"System.Data.DataCommonEventSource",
        //    //"Microsoft-Diagnostics-DiagnosticSource",
        //});
#endif
        while (true)
        {
            var sw = Stopwatch.StartNew();
#if !DEBUG
            try
            {
#endif
            Test1();
#if !DEBUG
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex?.GetTrue());
            }
#endif

            sw.Stop();
            Console.WriteLine("OK! 耗时 {0}", sw.Elapsed);
            //Thread.Sleep(5000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.C) break;
        }
    }

    static StarClient _client;
    private static void Test1()
    {
        var type = Type.GetType("UnityEngine.Application, UnityEngine");
        XTrace.WriteLine("UnityEngine：{0}", type?.FullName);

        var pool = Pool.Shared;
        var buf = pool.Rent(1000);

        buf[3] = 0x35;
        Thread.Sleep(1000);

        pool.Return(buf);
        pool.Return(buf);

        buf = pool.Rent(1000);
        XTrace.WriteLine(buf.ToHex("-", 0, 8));

        var buf2 = pool.Rent(800);
        XTrace.WriteLine(buf2.ToHex("-", 0, 8));

        buf[4] = 0x37;
        buf2[4] = 0x39;
        XTrace.WriteLine(buf.ToHex("-", 0, 8));
        XTrace.WriteLine(buf2.ToHex("-", 0, 8));

        //var client = new WebClientX { Log = XTrace.Log };
        //client.AuthKey = "NewLife";
        ////var rs = client.DownloadLink("http://sh03.newlifex.com,http://x.newlifex.com", "ip.gz", "tt/");
        ////var rs = client.DownloadLink("http://sh03.newlifex.com,http://x.newlifex.com", "leaf", "tt/");
        //var rs = client.DownloadLink("http://sh03.newlifex.com,https://x.newlifex.com/dotNet/8.0.7", "dotnet-runtime-8.0.7-linux-x64", "tt/");
        //XTrace.WriteLine(rs);
    }

    private static void Test2()
    {
        var sw = Stopwatch.StartNew();

        var count = 100_000_000L;

        var ts = new List<Task>();
        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            ts.Add(Task.Run(() =>
            {
                var f = new Snowflake();

                for (var i = 0; i < count; i++)
                {
                    var id = f.NewId();
                }
            }));
        }

        Task.WaitAll(ts.ToArray());

        sw.Stop();

        count *= ts.Count;

        XTrace.WriteLine("生成 {0:n0}，耗时 {1}，速度 {2:n0}tps", count, sw.Elapsed, count * 1000 / sw.ElapsedMilliseconds);

        Runtime.FreeMemory();
    }

    private static void Test3()
    {
        //var str = $"{DateTime.Now:yyyy}年，学无先后达者为师！";
        //str.SpeakAsync();

        XTrace.WriteLine("hello");
        Task.Run(() =>
        {
            XTrace.WriteLine("222");
            Task.Run(() =>
            {
                XTrace.WriteLine("333");
            });
        });

        var set = StarSetting.Current;
        set.Debug = true;
        var local = new LocalStarClient { Log = XTrace.Log };
        var info = local.GetInfo();
        XTrace.WriteLine("Info: {0}", info?.ToJson());

        var client3 = new ApiClient("udp://localhost:5500")
        {
            Timeout = 3_000,
            Log = XTrace.Log,
            EncoderLog = XTrace.Log,
        };
        info = client3.Invoke<AgentInfo>("info");
        XTrace.WriteLine("Info: {0}", info?.ToJson());

        var uri = new NetUri("http://sso.newlifex.com");
        var client = uri.CreateRemote();
        client.Log = XTrace.Log;
        client.LogSend = true;
        client.LogReceive = true;
        if (client is TcpSession tcp) tcp.MaxAsync = 0;
        client.Open();

        client.Send("GET /cube/info HTTP/1.1\r\nHost: sso.newlifex.com\r\n\r\n");

        var rs = client.ReceiveString();
        XTrace.WriteLine(rs);
    }

    private static void Test4()
    {
        // 提升进程优先级
        var p = Process.GetCurrentProcess();
        p.PriorityClass = ProcessPriorityClass.High;

        //var v = Rand.NextBytes(32);
        //Console.WriteLine(v.ToBase64());

        ICache ch = null;
        //ICache ch = new DbCache();
        //ch.Set(key, v);
        //v = ch.Get<Byte[]>(key);
        //Console.WriteLine(v.ToBase64());
        //ch.Remove(key);

        Console.Clear();

        Console.Write("选择要测试的缓存：1，MemoryCache；2，DbCache；3，Redis ");
        var select = Console.ReadKey().KeyChar;
        switch (select)
        {
            case '1':
                ch = new MemoryCache();
                break;
                //case '3':
                //    var rds = new Redis("127.0.0.1", null, 9)
                //    {
                //        Counter = new PerfCounter(),
                //        Tracer = new DefaultTracer { Log = XTrace.Log },
                //    };
                //    ch = rds;
                //    break;
        }

        var mode = false;
        Console.WriteLine();
        Console.Write("选择测试模式：1，顺序；2，随机 ");
        if (Console.ReadKey().KeyChar != '1') mode = true;

        var batch = 0;
        if (mode)
        {
            Console.WriteLine();
            Console.Write("选择输入批大小[0]：");
            batch = Console.ReadLine().ToInt();
        }

        Console.Clear();

        //var batch = 0;
        //if (mode) batch = 1000;

        var rs = ch.Bench(mode, batch);

        XTrace.WriteLine("总测试数据：{0:n0}", rs);
        //if (ch is Redis rds2) XTrace.WriteLine(rds2.Counter + "");

        Runtime.FreeMemory();
    }

    private static NetServer _server;
    private static async void Test5()
    {
        var provider = ObjectContainer.Provider;

        var server = new HttpServer
        {
            Port = 8080,
            ServiceProvider = provider,

            Log = XTrace.Log,
            //SessionLog = XTrace.Log,
        };
        server.Map("/", () => "<h1>Hello NewLife!</h1></br> " + DateTime.Now.ToFullString() + "</br><img src=\"logos/leaf.png\" />");
        server.Map("/user", (String act, Int32 uid) => new { code = 0, data = $"User.{act}({uid}) success!" });
        server.MapStaticFiles("/logos", "images/");
        //server.MapController<ApiController>("/api");
        server.MapController<MyHttpController>("/api");
        server.Map("/my", new MyHttpHandler());
        server.Map("/ws", new WebSocketHandler());
        server.MapStaticFiles("/", "./");
        server.Start();

        _server = server;

#if NET5_0_OR_GREATER
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://127.0.0.1:8080/ws"), default);
        await client.SendAsync("Hello NewLife".GetBytes(), System.Net.WebSockets.WebSocketMessageType.Text, true, default);

        var buf = new Byte[1024];
        var rs = await client.ReceiveAsync(buf, default);
        XTrace.WriteLine(new Packet(buf, 0, rs.Count).ToStr());

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "通信完成", default);
        XTrace.WriteLine("Close [{0}] {1}", client.CloseStatus, client.CloseStatusDescription);
#endif
    }

    private class MyHttpHandler : IHttpHandler
    {
        public void ProcessRequest(IHttpContext context)
        {
            var name = context.Parameters["name"];
            var html = $"<h2>你好，<span color=\"red\">{name}</span></h2>";
            var files = context.Request.Files;
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    file.SaveToFile();
                    html += $"<br />文件：{file.FileName} 大小：{file.Length} 类型：{file.ContentType}";
                }
            }
            context.Response.SetResult(html);
        }
    }

    private class MyHttpController
    {
        private readonly NetSession _session;

        public MyHttpController(NetSession session) => _session = session;

        public String Info() => $"你好 {_session.Remote}，现在时间是：{DateTime.Now.ToFullString()}";
    }

    private static void Test6()
    {
        XTrace.WriteLine("TLS加密通信");

        var pfx = new X509Certificate2("../../../doc/newlife.pfx".GetFullPath(), "newlife");
        //Console.WriteLine(pfx);

        //using var svr = new ApiServer(1234);
        //svr.Log = XTrace.Log;
        //svr.EncoderLog = XTrace.Log;

        //var ns = svr.EnsureCreate() as NetServer;

        using var ns = new NetServer(1234)
        {
            Name = "Server",
            ProtocolType = NetType.Tcp,
            //SslProtocol = SslProtocols.Tls12,
            Certificate = pfx,

            Log = XTrace.Log,
            SessionLog = XTrace.Log,
            SocketLog = XTrace.Log,
            LogReceive = true
        };

        //ns.EnsureCreateServer();
        //foreach (var item in ns.Servers)
        //{
        //    if (item is TcpServer ts) ts.Certificate = pfx;
        //}

        ns.Received += (s, e) =>
        {
            XTrace.WriteLine("收到：{0}", e.Packet.ToStr());
        };
        ns.Start();

        using var client = new TcpSession
        {
            Name = "Client",
            Remote = new NetUri("tcp://127.0.0.1:1234"),
            SslProtocol = SslProtocols.Tls12,
            Certificate = pfx,

            Log = XTrace.Log,
            LogSend = true
        };
        client.Open();

        client.Send("Stone");

        Console.ReadLine();
    }

    private static void Test7()
    {
        var fi = "D:\\Tools".AsDirectory().GetFiles().OrderByDescending(e => e.Length).FirstOrDefault();
        fi ??= "../../".AsDirectory().GetFiles().OrderByDescending(e => e.Length).FirstOrDefault();
        XTrace.WriteLine("发送文件：{0}", fi.FullName);
        XTrace.WriteLine("文件大小：{0}", fi.Length.ToGMK());

        var uri = new NetUri("tcp://127.0.0.3:12345");
        var client = uri.CreateRemote();
        client.Log = XTrace.Log;

        client.Add<StandardCodec>();
        client.Open();

        client.SendMessage($"Send File {fi.Name}");

        var rs = client.SendFile(fi.FullName);
        XTrace.WriteLine("分片：{0}", rs);

        client.SendMessage($"Send File Finished!");

        //Console.ReadKey();
    }

    private static void Test8()
    {
        foreach (var p in Process.GetProcessesByName("dotnet"))
        {
            //Runtime.FreeMemory(p.Id);
            var name = p.GetProcessName();
            XTrace.WriteLine("{0}\t{1}", p.Id, name);
        }
    }

    private static void Test9()
    {
        var ips = NetHelper.GetIPs().Where(e => e.IsIPv4()).ToList();
        XTrace.WriteLine(ips.Join(","));
    }

    private static void Test10()
    {
        var args = Environment.GetCommandLineArgs();
        if (args == null || args.Length < 2) return;

        XTrace.WriteLine(args[1]);

        var count = 10 * 1024 * 1024;
#if DEBUG
        count = 1024;
#endif
        var fi = args[1].AsFile();
        if (!fi.Exists || fi.Length < count) return;

        // 取最后1M
        using var fs = fi.OpenRead();
        var count2 = count;
        if (count2 > fs.Length) count2 = (Int32)fs.Length;
        //fs.Seek(count2, SeekOrigin.End);
        fs.Position = fs.Length - count2;

        var buf = fs.ReadBytes(-1);
        File.WriteAllBytes($"{DateTime.Now:yyyyMMddHHmmss}.log".GetFullPath(), buf);
    }

    private static void Test11()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < 26; i++)
        {
            sb.Append((Char)('a' + i));
        }
        for (var i = 0; i < 26; i++)
        {
            sb.Append((Char)('A' + i));
        }
        for (var i = 0; i < 10; i++)
        {
            sb.Append((Char)('0' + i));
        }
        Console.WriteLine(sb);
    }

    /// <summary>测试序列化</summary>
    private static void Test12()
    {
    }

    private static void Test13()
    {
        //DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(1024);

        ////var x = dsa.ExportCspBlob(true);

        //using (var fs = new FileStream("D:\\keys\\private.key", FileMode.Open, FileAccess.Read))
        //{
        //    var rs = new StreamReader(fs);
        //    var keystr = rs.ReadToEnd();
        //    DSAHelper.FromXmlStringX(dsa, keystr);

        //    DsaPublicKeyParameters dsaKey = DotNetUtilities.GetDsaPublicKey(dsa);
        //    using (StreamWriter sw = new StreamWriter("D:\\keys\\dsa.pem"))
        //    {
        //        PemWriter pw = new PemWriter(sw);
        //        pw.WriteObject(dsaKey);
        //    }
        //}
    }

    //private static void Test14()
    //{
    //    var rds = new Redis("127.0.0.1", null, 3)
    //    {
    //        Log = XTrace.Log
    //    };
    //    var rs = rds.Execute<Object>(null, rc => rc.Execute("XREAD", "count", "3", "streams", "stream_empty_item", "0-0"));
    //}

    ///// <summary>
    ///// 私钥XML2PEM
    ///// </summary>
    //private static void XMLConvertToPEM()//XML格式密钥转PEM
    //{
    //    var rsa2 = new RSACryptoServiceProvider();
    //    using (var sr = new StreamReader("D:\\keys\\private.key"))
    //    {
    //        rsa2.FromXmlString(sr.ReadToEnd());
    //    }
    //    var p = rsa2.ExportParameters(true);

    //    var key = new RsaPrivateCrtKeyParameters(
    //        new Org.BouncyCastle.Math.BigInteger(1, p.Modulus), new Org.BouncyCastle.Math.BigInteger(1, p.Exponent), new Org.BouncyCastle.Math.BigInteger(1, p.D),
    //        new Org.BouncyCastle.Math.BigInteger(1, p.P), new Org.BouncyCastle.Math.BigInteger(1, p.Q), new Org.BouncyCastle.Math.BigInteger(1, p.DP), new Org.BouncyCastle.Math.BigInteger(1, p.DQ),
    //        new Org.BouncyCastle.Math.BigInteger(1, p.InverseQ));

    //    using (var sw = new StreamWriter("D:\\keys\\PrivateKey.pem"))
    //    {
    //        var pemWriter = new PemWriter(sw);
    //        pemWriter.WriteObject(key);
    //    }
    //}

    private static void ExportPublicKeyToPEMFormat()
    {

        var rsa2 = new RSACryptoServiceProvider();
        using (var sr = new StreamReader("D:\\keys\\private.key"))
        {
            rsa2.FromXmlString(sr.ReadToEnd());
        }

        var str = ExportPublicKeyToPEMFormat(rsa2);

        using (var sw = new StreamWriter("D:\\keys\\PublicKey.pem"))
        {
            //var pemWriter = new PemWriter(sw);
            //pemWriter.WriteObject(str);
            sw.Write(str);
        }

    }

    public static String ExportPublicKeyToPEMFormat(RSACryptoServiceProvider csp)
    {
        TextWriter outputStream = new StringWriter();

        var parameters = csp.ExportParameters(false);
        using (var stream = new MemoryStream())
        {
            var writer = new BinaryWriter(stream);
            writer.Write((Byte)0x30); // SEQUENCE
            using (var innerStream = new MemoryStream())
            {
                var innerWriter = new BinaryWriter(innerStream);
                EncodeIntegerBigEndian(innerWriter, new Byte[] { 0x00 }); // Version
                EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent);

                //All Parameter Must Have Value so Set Other Parameter Value Whit Invalid Data  (for keeping Key Structure  use "parameters.Exponent" value for invalid data)
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.D
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.P
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.Q
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.DP
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.DQ
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.InverseQ

                var length = (Int32)innerStream.Length;
                EncodeLength(writer, length);
                writer.Write(innerStream.GetBuffer(), 0, length);
            }

            var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (Int32)stream.Length).ToCharArray();
            outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
            // Output as Base64 with lines chopped at 64 characters
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
            }
            outputStream.WriteLine("-----END PUBLIC KEY-----");

            return outputStream.ToString();

        }
    }

    private static void EncodeIntegerBigEndian(BinaryWriter stream, Byte[] value, Boolean forceUnsigned = true)
    {
        stream.Write((Byte)0x02); // INTEGER
        var prefixZeros = 0;
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] != 0) break;
            prefixZeros++;
        }
        if (value.Length - prefixZeros == 0)
        {
            EncodeLength(stream, 1);
            stream.Write((Byte)0);
        }
        else
        {
            if (forceUnsigned && value[prefixZeros] > 0x7f)
            {
                // Add a prefix zero to force unsigned if the MSB is 1
                EncodeLength(stream, value.Length - prefixZeros + 1);
                stream.Write((Byte)0);
            }
            else
            {
                EncodeLength(stream, value.Length - prefixZeros);
            }
            for (var i = prefixZeros; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }
        }
    }

    private static void EncodeLength(BinaryWriter stream, Int32 length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
        if (length < 0x80)
        {
            // Short form
            stream.Write((Byte)length);
        }
        else
        {
            // Long form
            var temp = length;
            var bytesRequired = 0;
            while (temp > 0)
            {
                temp >>= 8;
                bytesRequired++;
            }
            stream.Write((Byte)(bytesRequired | 0x80));
            for (var i = bytesRequired - 1; i >= 0; i--)
            {
                stream.Write((Byte)(length >> (8 * i) & 0xff));
            }
        }
    }

    // dsa xml 转 pem
    private static void DSAXML2PEM()
    {
        // 私钥转换
        var dsa = new DSACryptoServiceProvider();
        using (var fs = new FileStream("D:\\token.prvkey", FileMode.Open, FileAccess.Read))
        {
            var sr = new StreamReader(fs);
            dsa.FromXmlStringX(sr.ReadToEnd());
        }

        //// 私钥
        //var dsaKey = DotNetUtilities.GetDsaKeyPair(dsa);
        //using (var sw = new StreamWriter("D:\\dsaprv.pem"))
        //{
        //    var pw = new PemWriter(sw);
        //    pw.WriteObject(dsaKey.Private);
        //}
        //// 公钥
        //using (var sw = new StreamWriter("D:\\dsapub.pem"))
        //{
        //    var pw = new PemWriter(sw);
        //    pw.WriteObject(dsaKey.Public);
        //}


        //// 公钥转换
        //var pubdsa = new DSACryptoServiceProvider();
        //using (var fs = new FileStream("D:\\token.pubkey", FileMode.Open, FileAccess.Read))
        //{
        //    var sr = new StreamReader(fs);
        //    pubdsa.FromXmlStringX(sr.ReadToEnd());
        //}

        //var dsapub = DotNetUtilities.GetDsaPublicKey(pubdsa);
        //using (var sw = new StreamWriter("D:\\dsapub1.pem"))
        //{
        //    var pw = new PemWriter(sw);
        //    pw.WriteObject(dsapub);
        //}
    }

    //// dsa public pem 转 xml
    //private static void DSAPublicPEM2XML()
    //{
    //    DSA dsa;
    //    using (var rdr = new StreamReader("D:\\dsapub.pem"))
    //    {
    //        var pr = new PemReader(rdr);
    //        var o = pr.ReadObject() as DsaPublicKeyParameters;
    //        var prm = new CspParameters(13);
    //        prm.Flags = CspProviderFlags.UseMachineKeyStore;

    //        dsa = new DSACryptoServiceProvider(prm);
    //        var dp = new DSAParameters
    //        {
    //            G = o.Parameters.G.ToByteArrayUnsigned(),
    //            P = o.Parameters.P.ToByteArrayUnsigned(),
    //            Q = o.Parameters.Q.ToByteArrayUnsigned(),
    //            Y = o.Y.ToByteArrayUnsigned()
    //        };

    //        if (o.Parameters.ValidationParameters != null)
    //        {
    //            dp.Counter = o.Parameters.ValidationParameters.Counter;
    //            dp.Seed = o.Parameters.ValidationParameters.GetSeed();
    //        }

    //        dsa.ImportParameters(dp);
    //    }

    //    // 写入xml文件
    //    using (var fs = new FileStream("D:\\xtoken.pubkey", FileMode.Create, FileAccess.Write))
    //    {
    //        var sw = new StreamWriter(fs);

    //        var xml = dsa.ToXmlString(false);
    //        sw.Write(xml);
    //        sw.Flush();
    //        sw.Dispose();
    //    }
    //}

    //// dsa private pem 转 xml
    //private static void DSAPrivatePEM2XML()
    //{
    //    DSA prvDsa;
    //    DSA pubDsa;

    //    using (var rdr = new StreamReader("D:\\dsaprv.pem"))
    //    {
    //        var pr = new PemReader(rdr);
    //        var opair = pr.ReadObject() as AsymmetricCipherKeyPair;

    //        var prm = new CspParameters(13);
    //        prm.Flags = CspProviderFlags.UseMachineKeyStore;

    //        //var prm1 = new CspParameters(13);
    //        //prm1.Flags = CspProviderFlags.UseMachineKeyStore;

    //        prvDsa = new DSACryptoServiceProvider(prm);
    //        pubDsa = new DSACryptoServiceProvider(prm);

    //        // 私钥
    //        var prvpara = opair.Private as DsaPrivateKeyParameters;
    //        var prvdp = new DSAParameters
    //        {
    //            G = prvpara.Parameters.G.ToByteArrayUnsigned(),
    //            P = prvpara.Parameters.P.ToByteArrayUnsigned(),
    //            Q = prvpara.Parameters.Q.ToByteArrayUnsigned(),
    //            X = prvpara.X.ToByteArrayUnsigned()
    //        };
    //        if (prvpara.Parameters.ValidationParameters != null)
    //        {
    //            prvdp.Counter = prvpara.Parameters.ValidationParameters.Counter;
    //            prvdp.Seed = prvpara.Parameters.ValidationParameters.GetSeed();
    //        }
    //        prvDsa.ImportParameters(prvdp);

    //        // 公钥
    //        var pubpara = opair.Public as DsaPublicKeyParameters;
    //        var pubdp = new DSAParameters
    //        {
    //            G = pubpara.Parameters.G.ToByteArrayUnsigned(),
    //            P = pubpara.Parameters.P.ToByteArrayUnsigned(),
    //            Q = pubpara.Parameters.Q.ToByteArrayUnsigned(),
    //            Y = pubpara.Y.ToByteArrayUnsigned()
    //        };
    //        if (pubpara.Parameters.ValidationParameters != null)
    //        {
    //            pubdp.Counter = pubpara.Parameters.ValidationParameters.Counter;
    //            pubdp.Seed = pubpara.Parameters.ValidationParameters.GetSeed();
    //        }
    //        pubDsa.ImportParameters(pubdp);
    //    }

    //    // 写入xml文件 private
    //    using (var sw = new StreamWriter("D:\\xtoken.prvkey"))
    //    {
    //        //var sw = new StreamWriter(fs);

    //        var xml = prvDsa.ToXmlString(true);
    //        sw.Write(xml);
    //        sw.Flush();
    //        //sw.Dispose();
    //    }
    //    // 写入xml文件 public
    //    using (var fs = new FileStream("D:\\xtoken.pubkey", FileMode.Create, FileAccess.Write))
    //    {
    //        var sw = new StreamWriter(fs);
    //        var xml = pubDsa.ToXmlString(false);
    //        sw.Write(xml);
    //        sw.Flush();
    //        sw.Dispose();
    //    }
    //}

    // 测试加密
    private static void Test15()
    {
        Byte[] signStr;

        using (var prvfs = new FileStream("D:\\xtoken.prvkey", FileMode.Open, FileAccess.Read))
        {
            var sr = new StreamReader(prvfs);
            var prvdsa = new DSACryptoServiceProvider();
            prvdsa.FromXmlStringX(sr.ReadToEnd());

            signStr = prvdsa.SignData("123".GetBytes());
            Console.WriteLine("签名结果：" + signStr.ToBase64());
        }

        using (var pubfs = new FileStream("D:\\xtoken.pubkey", FileMode.Open, FileAccess.Read))
        {
            var sr = new StreamReader(pubfs);
            var pubdsa = new DSACryptoServiceProvider();
            pubdsa.FromXmlStringX(sr.ReadToEnd());

            var result = pubdsa.VerifyData("123".GetBytes(), signStr);
            Console.WriteLine("验证结果:" + result);
        }
    }

    private static void TestReadAppSettings()
    {
    }

    /// <summary>测试config文件的写入</summary>
    private static void TestWriteConfig()
    {
        ConfigTest.Current.Names = new List<String> { "1", "2" };
        ConfigTest.Current.Sex = "1";
        ConfigTest.Current.xyf = new List<XYF>() { new XYF() { name = "123" }, new XYF() { name = "321" } };
        ConfigTest.Current.Save();

        //Class1.Current.Names = "123";
        //Class1.Current.Save();

        //Class1.Provider = XmlConfig;


    }

    /// <summary>测试config文件的读取</summary>
    private static void TestReadConfig()
    {
        var z = ConfigTest.Current.Names;
        var x = ConfigTest.Current.Sex;
        var y = ConfigTest.Current.xyf;
    }

    private static void Test16()
    {
        using var fs = new FileStream("d:\\1233.csv", FileMode.OpenOrCreate, FileAccess.Read);

        var csv = new CsvFile(fs);

        var obj = csv.ReadLine();

        csv.Dispose();

    }

    /// <summary>测试写入CSV文件</summary>
    private static void Test17()
    {
        using var fs = new FileStream("d:\\1233.csv", FileMode.OpenOrCreate, FileAccess.Write);

        var csv = new CsvFile(fs);

        csv.WriteLine("123", true, 111111111, DateTime.Now.ToString("yyyyMMdd"), "2222222222222222222222222222");


        csv.Dispose();

    }
}