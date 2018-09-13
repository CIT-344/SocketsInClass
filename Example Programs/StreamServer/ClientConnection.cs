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
        public Task Writer { get; private set; }
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

        private BinaryWriter StreamWriter;
        private BinaryReader StreamReader;

        public ClientConnection(Socket Connection, Guid ClientID)
        {
            ConnectedAt = DateTime.Now;
            _Connection = Connection;
            this.ClientID = ClientID;

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

                while (IsConencted)
                {
                    // Keep Reading and waiting
                    var result = StreamReader.ReadString();
                    // Process this result
                    Console.WriteLine($"Client {ClientID.ToString()} said {result}");
                    // Go back to waiting for content
                }
            });
        }

        private void GetWriter()
        {
            Writer = Task.Factory.StartNew(() =>
            {
                StreamWriter = new BinaryWriter(new NetworkStream(_Connection, FileAccess.Write), Encoding.UTF8, true);
            });
        }


        public async Task SendAsync(String Msg)
        {
            if (IsConencted)
            {
                await Task.Factory.StartNew(()=> { StreamWriter.Write(Msg); });
            }
        }



    }
}
