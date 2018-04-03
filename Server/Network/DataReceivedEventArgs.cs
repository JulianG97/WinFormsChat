using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class DataReceivedEventArgs
    {
        public DataReceivedEventArgs(byte[] data, TcpClient client)
        {
            this.Protocol = new Protocol(data);
            this.Client = client;
        }

        public Protocol Protocol
        {
            get;
            private set;
        }

        public TcpClient Client
        {
            get;
            private set;
        }
    }
}