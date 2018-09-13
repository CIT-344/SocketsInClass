using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StreamClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket _client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            _client.Connect(IPAddress.Loopback, 11000);


            using (var bWriter = new BinaryWriter(new NetworkStream(_client, FileAccess.ReadWrite), Encoding.UTF8, true))
            {
                while (_client.Connected)
                {
                    Console.WriteLine("Enter text to send");
                    bWriter.Write(Console.ReadLine());
                }
            }


            Console.WriteLine("Disconnected from Server");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
