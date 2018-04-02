using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class User
    {
        public User(string username, TcpClient client)
        {
            this.Username = username;
            this.Client = client;
        }

        public string Username
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
