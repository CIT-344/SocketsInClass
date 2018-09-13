using IStreaming.Shared_Models;
using Streaming.Shared_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamServer
{
    public delegate void ConnectionHandler(ClientConnection sender, bool IsDisconnect);
    public delegate void InBoundMessageHandler(ClientConnection Connection,Communication_Model Data);
    class Program: IEventStorage
    {
        static Socket _server;

        static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static List<ClientConnection> Clients = new List<ClientConnection>();

        static event ConnectionHandler OnClientDisconnect;
        static event InBoundMessageHandler OnClientMessage;

        
        
        static void Main(string[] args)
        {
            Console.Title = "Server";

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            _server = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndPoint);
            _server.Listen(10);

            OnClientDisconnect += ClientConnectionEvent;
            OnClientMessage += ClientMessageReceieved;

            while (!TokenSource.IsCancellationRequested)
            {
                var _client = _server.Accept();
                var ClientConnection = new ClientConnection(_client, Guid.NewGuid(), OnClientDisconnect, OnClientMessage);
                
            }
        }

        private static void ClientMessageReceieved(ClientConnection Connection,Communication_Model Data)
        {
            throw new NotImplementedException();
        }

        private async static void ClientConnectionEvent(ClientConnection Sender, bool IsDisconnect)
        {
            if (IsDisconnect)
            {
                Clients.RemoveAll(x => x.ClientID == Sender.ClientID);
                Console.WriteLine($"Client {Sender.ClientID.ToString()} has disconnected at {DateTime.Now.ToShortTimeString()}");
                await BroadcastDisconnectEvent(Sender);
            }
            else
            {
                Clients.Add(Sender);
                Console.WriteLine($"Client {Sender.ClientID.ToString()} has connected at {DateTime.Now.ToShortTimeString()}");
                await BroadcastConnectedEvent(Sender);
            }
        }
        

        static async Task BroadcastDisconnectEvent(ClientConnection Client)
        {
            foreach (var client in Clients)
            {
                // Send all a message
                await client.SendAsync("DISCONNECT",$"Client {Client.ClientID.ToString()} has connected");
            }
        }

        static async Task BroadcastConnectedEvent(ClientConnection Client)
        {
            foreach (var client in Clients)
            {
                // Send all a message
                await client.SendAsync("CONNECT",$"Client {Client.ClientID.ToString()} has connected");
            }
        }
    }
}
