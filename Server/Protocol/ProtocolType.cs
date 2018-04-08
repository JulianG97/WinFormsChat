using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ProtocolType
    {
        public static byte[] LogIn = new byte[] { 76, 73 };
        public static byte[] LogOut = new byte[] { 76, 79 };
        public static byte[] IsAlive = new byte[] { 73, 65 };
        public static byte[] AddUser = new byte[] { 65, 85 };
        public static byte[] RemoveUser = new byte[] { 82, 85 };
        public static byte[] SessionKey = new byte[] { 83, 69 };
        public static byte[] SessionKeyReceived = new byte[] { 83, 82 };
        public static byte[] Message = new byte[] { 77, 69 };
        public static byte[] NewMessage = new byte[] { 78, 77 };
    }
}
