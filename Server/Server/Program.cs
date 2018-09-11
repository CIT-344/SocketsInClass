using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        
        static Socket _server;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server on Port");
            StartServer();
        }


        static void StartServer()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            _server = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndPoint);
            _server.Listen(10);

            var client = _server.Accept();

            while (true)
            {
                var buffer = new byte[1024];

                client.Receive(buffer, SocketFlags.None);
                var number = Encoding.UTF8.GetString(buffer).Trim();
                var numbers = number.Split(' ');
                var result = Convert.ToInt32(numbers[0]) + Convert.ToInt32(numbers[1]);
                client.Send(Encoding.UTF8.GetBytes(result.ToString()), SocketFlags.None);
            }
        }
    }
}
