using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Streaming.Shared_Models;

namespace StreamClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Socket _client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            _client.Connect(IPAddress.Loopback, 11000);

            Task.Factory.StartNew(()=> 
            {
                StartReader(_client);
            });

            using (var bWriter = new BinaryWriter(new NetworkStream(_client, FileAccess.ReadWrite), Encoding.UTF8, true))
            {
                while (_client.Connected)
                {
                    Console.WriteLine("Enter text to send");
                    var line = Console.ReadLine();

                    switch (line)
                    {
                        case "/exit":
                            _client.Disconnect(false);
                            break;
                        default:
                            bWriter.WriteDataModel(new Communication_Model("Msg", line));
                            break;
                    }

                    
                }
            }
            Console.WriteLine("Disconnected from Server");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void StartReader(Socket Server)
        {
            if (Server.Connected)
            {
                try
                {
                    using (var StreamReader = new BinaryReader(new NetworkStream(Server), Encoding.UTF8, true))
                    {
                        while (Server.Connected)
                        {
                            // Keep Reading and waiting
                            var result = StreamReader.ReadDataModel();
                            Console.WriteLine($"Server said {result.Body}");
                        }
                    }
                }
                catch (EndOfStreamException disconnect)
                {
                    
                }
            }
        }
    }
}
