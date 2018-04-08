using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class NetworkWatcher
    {
        private TcpClient client;
        private NetworkStream stream;
        private IPEndPoint ipEndPoint;
        private bool isReading;
        private Thread readThread;
        private Thread isAliveThread;

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<EventArgs> ConnectionLost;

        public NetworkWatcher(IPAddress ip, int port)
        {
            this.ipEndPoint = new IPEndPoint(ip, port);
            this.client = new TcpClient();
        }

        public bool Connected
        {
            get;
            private set;
        }

        public void Start()
        {
            try
            {
                if (this.client.ConnectAsync(this.ipEndPoint.Address, this.ipEndPoint.Port).Wait(1000) == false)
                {
                    throw new TimeoutException();
                }

                this.stream = this.client.GetStream();

                this.readThread = new Thread(Read2);
                this.isReading = true;
                this.readThread.Start();
                this.Connected = true;

                this.isAliveThread = new Thread(this.IsAliveWorker);
                this.isAliveThread.Priority = ThreadPriority.Highest;
                this.isAliveThread.Start();
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        private void IsAliveWorker()
        {
            while (this.isReading == true)
            {
                this.Send(ProtocolCreator.IsAlive());

                Thread.Sleep(2000);
            }
        }

        public void Stop()
        {
            try
            {
                this.isReading = false;
                this.stream.Close();
                this.client.Close();
                this.client = new TcpClient();
                this.Connected = false;
            }
            catch
            {

            }
        }

        private void Read()
        {
            while (this.isReading == true)
            {
                /*if (this.stream.DataAvailable == false)
                {
                    Thread.Sleep(10);
                    continue;
                }*/

                List<byte> receivedBytes = new List<byte>();
                byte[] buffer = new byte[1];

                while (this.stream.DataAvailable == true)
                {
                    this.stream.Read(buffer, 0, 1);

                    if (buffer[0] == 127)
                    {
                        break;
                    }

                    receivedBytes.Add(buffer[0]);
                }

                if (receivedBytes.Count >= 6)
                {
                    if (receivedBytes[0] == 67 && receivedBytes[1] == 72 && receivedBytes[2] == 65 && receivedBytes[3] == 84)
                    {
                        if (receivedBytes[4] == 73 && receivedBytes[5] == 65)
                        {
                            continue;
                        }

                        this.FireOnDataReceived(receivedBytes.ToArray());

                    }
                }
            }
        }

        private void Read2()
        {
            while (true)
            {
                try
                {
                    var received = new List<byte>();
                    var buffer = new byte[1];
                    int len = 0;

                    while (buffer[0] != 127)
                    {
                        len = this.stream.Read(buffer, 0, 1);
                        received.Add(buffer[0]);
                    }

                    this.FireOnDataReceived(received.ToArray());
                }
                catch
                {
                    this.FireOnConnectionLost();
                }
            }
        }

        protected virtual void FireOnDataReceived(byte[] data)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, new DataReceivedEventArgs(data, this.client));
            }
        }

        protected virtual void FireOnConnectionLost()
        {
            if (this.ConnectionLost != null)
            {
                this.ConnectionLost(this, new EventArgs());
            }
        }

        public void Send(Protocol protocol)
        {
            try
            {
                byte[] sendBytes = protocol.ToByteArray();

                this.stream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }
    }
}
