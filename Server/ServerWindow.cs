using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public partial class ServerWindow : Form
    {
        private bool isRunning;
        private List<User> users;
        private TcpListener listener;
        private Thread listenerThread;

        public ServerWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (this.isRunning == true)
            {
                MessageBox.Show("The server is already running!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                int port = CheckPort(this.portTextBox.Text);

                if (port == 0)
                {
                    MessageBox.Show("The port must be an integer between 3000 and 9000!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    try
                    {
                        this.isRunning = true;
                        this.portTextBox.Enabled = false;
                        this.ipAddressTextBox.Text = this.GetExternalIPAddress().ToString();

                        this.users = new List<User>();

                        this.listener = new TcpListener(IPAddress.Any, port);
                        this.listener.Start();

                        this.listenerThread = new Thread(this.AcceptConnections);
                        this.listenerThread.Start();

                        this.AddLineToLog("The server was successfully started!");
                    }
                    catch
                    {
                        this.isRunning = false;
                        this.portTextBox.Enabled = true;
                        this.ipAddressTextBox.Text = string.Empty;

                        if (this.listener != null)
                        {
                            this.listener.Stop();
                        }

                        MessageBox.Show("The server couldn't be started!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }

        private IPAddress GetExternalIPAddress()
        {
            string externalIPString = new WebClient().DownloadString("http://icanhazip.com");

            char[] externalIPArray = externalIPString.ToCharArray();

            string externalIP = string.Empty;

            for (int i = 0; i < externalIPArray.Length; i++)
            {
                if (externalIPArray[i] == '\n')
                {
                    continue;
                }

                externalIP = externalIP + externalIPArray[i];
            }

            return IPAddress.Parse(externalIP);
        }

        private int CheckPort(string portString)
        {
            if (int.TryParse(portString, out int port) == true)
            {
                if (port >= 3000 && port <= 9000)
                {
                    return port;
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        private void AcceptConnections()
        {
            while (this.isRunning == true)
            {
                if (this.listener.Pending() == false)
                {
                    Thread.Sleep(100);
                    continue;
                }

                TcpClient client = this.listener.AcceptTcpClient();
                Thread session = new Thread(new ParameterizedThreadStart(HandleNewSession));
                session.Start(client);
            }
        }

        private void AddLineToLog(string line)
        {
            string[] newLog = new string[this.logRichTextBox.Lines.Length + 1];

            for (int i = 0; i < this.logRichTextBox.Lines.Length; i++)
            {
                newLog[i] = this.logRichTextBox.Lines[i];
            }

            newLog[this.logRichTextBox.Lines.Length] = line;

            this.logRichTextBox.Lines = newLog;
        }

        private void HandleNewSession(object data)
        {
            TcpClient client = (TcpClient)data;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (this.isRunning == false)
            {
                MessageBox.Show("The server isn't running!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                this.listener.Stop();
                this.isRunning = false;
                this.ipAddressTextBox.Text = string.Empty;
                this.portTextBox.Enabled = true;
                this.AddLineToLog("The server was successfully stopped!");
            }
        }
    }
}
