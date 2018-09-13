using System;
using System.Collections.Generic;
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

        static List<ClientConnection> Clients = new List<ClientConnection>();

        
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
                var ClientConnection = new ClientConnection(_client, Guid.NewGuid());
                Clients.Add(ClientConnection);

                BroadcastConnectedEvent(ClientConnection).Wait();
            }
        }


        static async Task BroadcastConnectedEvent(ClientConnection Client)
        {
            foreach (var client in Clients)
            {
                // Send all a message
                await client.SendAsync($"Client {Client.ClientID.ToString()} has connected");
            }
        }
    }
}
