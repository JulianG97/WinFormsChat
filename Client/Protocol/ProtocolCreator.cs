using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ProtocolCreator
    {
        private static Random random = new Random();
        private static object locker = new object();

        public static Protocol IsAlive()
        {
            Protocol protocol = new Protocol(ProtocolType.IsAlive, new byte[0]);
            return protocol;
        }

        public static Protocol LogIn(string username)
        {
            Protocol protocol = new Protocol(ProtocolType.LogIn, Encoding.ASCII.GetBytes(username));
            return protocol;
        }

        public static Protocol LogOut(string username, string sessionkey)
        {
            Protocol protocol = new Protocol(ProtocolType.LogOut, Encoding.ASCII.GetBytes(username + "-" + sessionkey));
            return protocol;
        }

        public static Protocol Message(string message, string sessionkey)
        {
            Protocol protocol = new Protocol(ProtocolType.Message, Encoding.ASCII.GetBytes(message + "-" + sessionkey));
            return protocol;
        }
    }
}
