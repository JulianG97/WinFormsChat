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
        public User(string username, NetworkWatcher networkWatcher, string sessionkey)
        {
            this.Username = username;
            this.NetworkWatcher = networkWatcher;
            this.SessionKey = sessionkey;
        }

        public string Username
        {
            get;
            private set;
        }

        public NetworkWatcher NetworkWatcher
        {
            get;
            private set;
        }

        public string SessionKey
        {
            get;
            private set;
        }
    }
}
