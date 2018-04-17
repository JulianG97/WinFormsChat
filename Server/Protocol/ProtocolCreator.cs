using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
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

        public static Protocol AddUser(string username)
        {
            Protocol protocol = new Protocol(ProtocolType.AddUser, Encoding.UTF8.GetBytes(username));
            return protocol;
        }

        public static Protocol RemoveUser(string username)
        {
            Protocol protocol = new Protocol(ProtocolType.RemoveUser, Encoding.UTF8.GetBytes(username));
            return protocol;
        }

        public static Protocol SessionKey()
        {
            Protocol protocol = new Protocol(ProtocolType.SessionKey, GenerateSessionKey());
            return protocol;
        }

        public static Protocol NewMessage(string message)
        {
            string time = DateTime.Now.ToString("[HH:mm]");

            Protocol protocol = new Protocol(ProtocolType.NewMessage, Encoding.UTF8.GetBytes(time + " " + message));
            return protocol;
        }

        public static byte[] GenerateSessionKey()
        {
            byte[] sessionKey = new byte[32];

            int number1;
            int number2;

            for (int i = 0; i < 32; i++)
            {
                lock (locker)
                {
                    number1 = random.Next(1, 4);
                }

                // Character will be a letter
                if (number1 == 1)
                {
                    lock (locker)
                    {
                        number2 = random.Next(1, 3);
                    }

                    // Letter will be upper case
                    if (number2 == 1)
                    {
                        lock (locker)
                        {
                            sessionKey[i] = (byte)random.Next(65, 91);
                        }
                    }
                    // Letter will be lower case
                    else if (number2 == 2)
                    {
                        lock (locker)
                        {
                            sessionKey[i] = (byte)random.Next(97, 123);
                        }
                    }
                }
                // Character will be a number
                else if (number1 == 2)
                {
                    lock (locker)
                    {
                        sessionKey[i] = (byte)random.Next(48, 58);
                    }
                }
                // Character will be a special character
                else if (number1 == 3)
                {
                    lock (locker)
                    {
                        sessionKey[i] = (byte)random.Next(33, 39);
                    }
                }
            }

            return sessionKey;
        }
    }
}
