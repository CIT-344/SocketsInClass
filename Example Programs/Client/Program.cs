using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/*
 * Cody Fraker
 * Dave Vanaman
 * Bailey Miller
 */

namespace Client
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Trying to connect to server");

            try
            {
                StartClient();
            }
            catch (Exception)
            {
                Console.WriteLine("An error has occured, press any key to exit.");
                Console.ReadKey();
            }
        }

        static void StartClient()
        {
            var buffer = new byte[100];

            Socket _client = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            _client.Connect(IPAddress.Loopback, 11000);

            while (true)
            {
                Console.WriteLine("Enter two numbers separted by a space");
                var numbers = Console.ReadLine();
                _client.Send(Encoding.UTF8.GetBytes(numbers.Trim()), SocketFlags.None);
                var bytesSent = _client.Receive(buffer, SocketFlags.None);
                var answer = Encoding.UTF8.GetString(buffer.Take(bytesSent).ToArray());
                Console.WriteLine($"Result was: {answer}");
            }
        }
    }
}
