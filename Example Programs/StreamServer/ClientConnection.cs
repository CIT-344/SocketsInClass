using Streaming.Shared_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer
{
    public class ClientConnection
    {
        public Task Reader { get; private set; }
        public readonly Guid ClientID;
        public readonly DateTime ConnectedAt;

        public bool IsConencted
        {
            get
            {
                if (_Connection != null)
                {
                    return _Connection.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        private readonly Socket _Connection;
        private readonly ConnectionHandler ConnectionEvent;
        private readonly InBoundMessageHandler MessageEvent;
        private BinaryWriter StreamWriter;
        private BinaryReader StreamReader;

        public ClientConnection(Socket Connection, Guid ClientID, ConnectionHandler ConnectionHandler, InBoundMessageHandler MessageHandler)
        {
            ConnectedAt = DateTime.Now;
            _Connection = Connection;
            this.ClientID = ClientID;
            ConnectionEvent = ConnectionHandler;
            this.MessageEvent = MessageHandler;
            GetWriter();
            GetReader();
        }


        private void GetReader()
        {
            Reader = Task.Factory.StartNew(()=> 
            {
                if (IsConencted)
                {
                    StreamReader = new BinaryReader(new NetworkStream(_Connection, FileAccess.Read), Encoding.UTF8, true);
                }

                try
                {
                    ConnectionEvent?.Invoke(this, false);
                    while (IsConencted)
                    {
                        // Keep Reading and waiting
                        var result = StreamReader.ReadDataModel();
                        MessageEvent?.Invoke(this, result);
                        // Process this result
                        Console.WriteLine($"Client {ClientID.ToString()} said {result}");
                        // Go back to waiting for content
                    }
                }
                catch (EndOfStreamException disconnected)
                {
                    _Connection.Close();
                    // The user has disconnected from the stream
                    ConnectionEvent?.Invoke(this, true);

                }
            });
        }

        private void GetWriter()
        {
            Task.Factory.StartNew(() =>
            {
                StreamWriter = new BinaryWriter(new NetworkStream(_Connection, FileAccess.Write), Encoding.UTF8, true);
            });
        }


        public async Task SendAsync(String EventName, Object Msg)
        {
            if (IsConencted)
            {
                await Task.Factory.StartNew(()=> 
                {
                    StreamWriter.WriteDataModel(new Communication_Model(EventName, Msg));
                });
            }
        }
    }

    
}
