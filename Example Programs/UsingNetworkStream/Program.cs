using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsingNetworkStream
{
    class Program
    {

        static Socket _server;

        static AutoResetEvent EndClientRequest = new AutoResetEvent(false);

        static CancellationTokenSource TokenSource = new CancellationTokenSource();
        

        static void Main(string[] args)
        {
            Console.WriteLine("Socket Communication using the NetworkStream class");

            var ServerThread = StartServer();

            var ClientThread = StartClient();

            // Wait for Close

            Task.WaitAny(ServerThread, ClientThread);
        }


        public static Task StartServer()
        {
            return Task.Factory.StartNew(() => 
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

                _server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                _server.Bind(localEndPoint);
                _server.Listen(10);

                
                while (!TokenSource.IsCancellationRequested)
                {
                    var _client = _server.Accept();

                    using (var bReader = new BinaryReader(new NetworkStream(_client, FileAccess.ReadWrite), Encoding.UTF8, true))
                    {
                        while (_client.Connected)
                        {
                            Console.WriteLine($"Client Said: {bReader.ReadString()}");
                        }
                    }
                }
                
            }, TokenSource.Token);
        }

        public static Task StartClient()
        {
            return Task.Factory.StartNew(() =>
            {
                Socket _client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

                _client.Connect(IPAddress.Loopback, 11000);


                using (var bWriter = new BinaryWriter(new NetworkStream(_client, FileAccess.ReadWrite), Encoding.UTF8, true))
                {
                    while (!TokenSource.IsCancellationRequested && _client.Connected)
                    {
                        Console.WriteLine("Enter text to send");
                        bWriter.Write(Console.ReadLine());
                    }
                }

                

            }, TokenSource.Token);
        }
    }
}
