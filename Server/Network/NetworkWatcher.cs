using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class NetworkWatcher
    {
        private NetworkStream stream;
        private TcpClient client;
        private bool isReading;
        private Thread readThread;

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

        public NetworkWatcher(TcpClient client)
        {
            this.client = client;
        }

        public void Start()
        {
            try
            {
                this.stream = this.client.GetStream();
                this.readThread = new Thread(this.Read);
                this.isReading = true;
                this.readThread.Start();
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        public void Stop()
        {
            try
            {
                this.isReading = false;
                this.stream.Close();
                this.client.Close();
            }
            catch
            {

            }
        }

        private void Read()
        {
            while (this.isReading == true)
            {
                if (this.stream.DataAvailable == false)
                {
                    Thread.Sleep(10);
                    continue;
                }

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

                this.FireOnDataReceived(receivedBytes.ToArray());
            }
        }

        protected virtual void FireOnDataReceived(byte[] data)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, new DataReceivedEventArgs(data));
            }
        }

        protected virtual void FireOnConnectionLost()
        {
            if (this.ConnectionLost != null)
            {
                this.ConnectionLost(this, new ConnectionLostEventArgs(this.client));
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
