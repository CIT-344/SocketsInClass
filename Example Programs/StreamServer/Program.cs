using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamServer
{
    class Program
    {
        static Socket _server;

        static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            _server = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndPoint);
            _server.Listen(10);


            while (!TokenSource.IsCancellationRequested)
            {
                var _client = _server.Accept();

                Task.Factory.StartNew(() => 
                {
                    StartClientListener(_client);
                }, TokenSource.Token);
            }
        }

        static void StartClientListener(Socket _client)
        {
            using (var bReader = new BinaryReader(new NetworkStream(_client, FileAccess.ReadWrite), Encoding.UTF8, true))
            {
                while (_client.Connected)
                {
                    Console.WriteLine($"Client Said: {bReader.ReadString()}");
                }
            }
        }
    }
}
