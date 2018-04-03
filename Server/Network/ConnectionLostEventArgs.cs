using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class ConnectionLostEventArgs
    {
        public ConnectionLostEventArgs(TcpClient client)
        {
            this.Client = client;
        }

        public TcpClient Client
        {
            get;
            private set;
        }
    }
}