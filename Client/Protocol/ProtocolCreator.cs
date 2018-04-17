using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ProtocolCreator
    {
        public static Protocol IsAlive()
        {
            Protocol protocol = new Protocol(ProtocolType.IsAlive, new byte[0]);
            return protocol;
        }

        public static Protocol LogIn(string username)
        {
            Protocol protocol = new Protocol(ProtocolType.LogIn, Encoding.UTF8.GetBytes(username));
            return protocol;
        }

        public static Protocol SessionKeyReceived(string username, string sessionkey)
        {
            Protocol protocol = new Protocol(ProtocolType.SessionKeyReceived, Encoding.UTF8.GetBytes(username + "-" + sessionkey));
            return protocol;
        }

        public static Protocol LogOut(string username, string sessionkey)
        {
            Protocol protocol = new Protocol(ProtocolType.LogOut, Encoding.UTF8.GetBytes(username + "-" + sessionkey));
            return protocol;
        }

        public static Protocol Message(string username, string message, string sessionkey)
        {
            Protocol protocol = new Protocol(ProtocolType.Message, Encoding.UTF8.GetBytes(username + "-" + message + "-" + sessionkey));
            return protocol;
        }
    }
}
