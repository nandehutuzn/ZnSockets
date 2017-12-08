using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets.TestSocketClient
{
    class Program
    {
        static TcpSocketClient _client;

        static void Main(string[] args)
        {
            ConnectServer();

            Console.WriteLine("APM TCP client has connected to server.");
            Console.WriteLine("Type something to send to server...");

            while (true)
            {
                try
                {
                    string text = Console.ReadLine();
                    if (text == "quit")
                    {
                        break;
                    }
                    else if (text == "many")
                    {
                        text = new string('x', 8192);
                        for (int i = 0; i < 1000000; i++)
                        {
                            _client.Send(Encoding.UTF8.GetBytes(text));
                        }
                    }
                    else if (text == "big1k")
                    {
                        text = new string('x', 1024 * 1);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big10k")
                    {
                        text = new string('x', 1024 * 10);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big100k")
                    {
                        text = new string('x', 1024 * 100);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big1m")
                    {
                        text = new string('x', 1024 * 1024 * 1);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big2m")
                    {
                        text = new string('x', 1024 * 1024 * 2);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big5m")
                    {
                        text = new string('x', 1024 * 1024 * 5);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big10m")
                    {
                        text = new string('x', 1024 * 1024 * 10);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big20m")
                    {
                        text = new string('x', 1024 * 1024 * 20);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big50m")
                    {
                        text = new string('x', 1024 * 1024 * 50);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big100m")
                    {
                        text = new string('x', 1024 * 1024 * 100);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else if (text == "big1g")
                    {
                        text = new string('x', 1024 * 1024 * 1024);
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                    else
                    {
                        _client.Send(Encoding.UTF8.GetBytes(text));
                    }
                }
                catch(Exception ex) { Console.WriteLine(ex.Message); }
            }

            _client.Shutdown();
            Console.WriteLine("TCP client has disconnected from server.");

            Console.ReadKey();
        }

        private static void ConnectServer()
        {
            var config = new TcpSocketClientConfiguration();
            //config.UseSsl = true;
            //config.SslTargetHost = "Cowboy";
            //config.SslClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\\Cowboy.cer"));
            //config.SslPolicyErrorsBypassed = false;
            //config.SendTimeout = TimeSpan.FromSeconds(2);

            //config.FrameBuilder = new FixedLengthFrameBuilder(20000);
            //config.FrameBuilder = new RawBufferFrameBuilder();
            //config.FrameBuilder = new LineBasedFrameBuilder();
            //config.FrameBuilder = new LengthPrefixedFrameBuilder();
            //config.FrameBuilder = new LengthFieldBasedFrameBuilder();

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 22222);

            _client = new TcpSocketClient(remoteEP, config);
            _client.ServerConnected += _client_ServerConnected;
            _client.ServerDisconnected += _client_ServerDisconnected;
            _client.ServerDataReceived += _client_ServerDataReceived;
            _client.Connect();
        }

        private static void _client_ServerDataReceived(object sender, TcpServerDataReceivedEventArgs e)
        {
            var text = Encoding.UTF8.GetString(e.Data, e.DataOffset, e.DataLength);
            Console.Write(string.Format("Server : {0} 服务发来消息--> ", e.Client.RemoteEndPoint));
            if (e.DataLength < 1024 * 1024 * 1)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.WriteLine("{0} Bytes", e.DataLength);
            }
        }

        private static void _client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            Console.WriteLine(string.Format("TCP server {0} has disconnected. 断开连接", e.RemoteEndPoint));
        }

        private static void _client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Console.WriteLine(string.Format("TCP server {0} has connected. 连接上服务", e.RemoteEndPoint));
        }
    }
}
