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
        private static object usersLocker;
        private static object logLocker;

        public ServerWindow()
        {
            InitializeComponent();
            logLocker = new object();
            this.FormClosing += this.CleanClosing;
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
                        usersLocker = new object();
                        this.portTextBox.Enabled = false;
                        this.ipAddressPubTextBox.Text = this.GetExternalIPAddress().ToString();
                        this.ipAddressPrivTextBox.Text = this.GetInternalIPAddress().ToString();

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
                        this.ipAddressPubTextBox.Text = string.Empty;
                        this.ipAddressPrivTextBox.Text = string.Empty;

                        if (this.listener != null)
                        {
                            this.listener.Stop();
                        }

                        MessageBox.Show("The server couldn't be started!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }

        private void CleanClosing(object sender, FormClosingEventArgs args)
        {
            if (this.isRunning == true)
            {
                DialogResult askIfClosing = MessageBox.Show("The server is running. Do you really want to exit?", "Chat - Server", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (askIfClosing == DialogResult.No)
                {
                    args.Cancel = true;
                }
                else
                {
                    this.StopButton_Click(this, new EventArgs());
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

        private IPAddress GetInternalIPAddress()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in localIPs)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return IPAddress.Parse("0.0.0.0");
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
            lock (logLocker)
            {
                MethodInvoker methodInvokerDelegate = delegate ()
                {
                    string time = DateTime.Now.ToString("[HH:mm]");

                    string[] newLog = new string[this.logRichTextBox.Lines.Length + 1];

                    for (int i = 0; i < this.logRichTextBox.Lines.Length; i++)
                    {
                        newLog[i] = this.logRichTextBox.Lines[i];
                    }

                    newLog[this.logRichTextBox.Lines.Length] = time + " " + line;

                    this.logRichTextBox.Lines = newLog;

                    this.logRichTextBox.SelectionStart = this.logRichTextBox.Text.Length;
                    this.logRichTextBox.ScrollToCaret();
                };

                if (this.InvokeRequired)
                {
                    this.Invoke(methodInvokerDelegate);
                }
                else
                {
                    methodInvokerDelegate();
                }
            }
        }

        private void HandleNewSession(object data)
        {
            TcpClient client = (TcpClient)data;
            NetworkWatcher networkWatcher = new NetworkWatcher(client);
            networkWatcher.ConnectionLost += this.ConnectionLost;
            networkWatcher.DataReceived += this.DataReceived;
            networkWatcher.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (this.isRunning == false)
            {
                MessageBox.Show("The server isn't running!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                this.isRunning = false;
                this.listener.Stop();

                // Close all open user connections
                lock (usersLocker)
                {
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        this.users[i].NetworkWatcher.Stop();
                    }
                }
             
                this.ipAddressPubTextBox.Text = string.Empty;
                this.ipAddressPrivTextBox.Text = string.Empty;
                this.portTextBox.Enabled = true;
                this.AddLineToLog("The server was successfully stopped!");
            }
        }

        private bool CheckIfLegalUsername(string username)
        {
            lock (usersLocker)
            {
                if (username.Length >= 3 && username.Length <= 10 && username != "SERVER")
                {
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        if (this.users[i].Username == username)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }

        private void CreateNewUser(string username, string sessionkey, NetworkWatcher networkWatcher)
        {
            lock (usersLocker)
            {
                this.users.Add(new User(username, networkWatcher, sessionkey));
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Protocol.Type.SequenceEqual(ProtocolType.LogIn))
            {
                if (args.Protocol.Content != null && args.Protocol.Content.Length >= 1)
                {
                    string username = Encoding.UTF8.GetString(args.Protocol.Content);
                    this.AddUser(username, (NetworkWatcher)sender);
                }
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.SessionKeyReceived))
            {
                if (args.Protocol.Content != null && args.Protocol.Content.Length >= 1)
                {
                    string usernameAndSessionKey = Encoding.UTF8.GetString(args.Protocol.Content);

                    string[] usernameAndSessionKeyArray = usernameAndSessionKey.Split('-');

                    if (usernameAndSessionKeyArray.Length == 2)
                    {
                        this.FinishUserLogin(usernameAndSessionKeyArray[0], usernameAndSessionKeyArray[1]);
                    }
                }
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.LogOut))
            {
                if (args.Protocol.Content != null && args.Protocol.Content.Length >= 1)
                {
                    string usernameAndSessionKey = Encoding.UTF8.GetString(args.Protocol.Content);

                    string[] usernameAndSessionKeyArray = usernameAndSessionKey.Split('-');

                    if (usernameAndSessionKeyArray.Length == 2)
                    {
                        this.RemoveUser(usernameAndSessionKeyArray[0], usernameAndSessionKeyArray[1]);
                    }
                }
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.Message))
            {
                if (args.Protocol.Content != null && args.Protocol.Content.Length >= 1)
                {
                    string message = Encoding.UTF8.GetString(args.Protocol.Content);
                    this.ForwardMessageToAllUsers(message);
                }
            }
        }

        private void RemoveUser(string username, string sessionkey)
        {
            lock (usersLocker)
            {
                // Verifys session key of the logout request
                bool legitSessionKey = false;

                for (int i = 0; i < this.users.Count; i++)
                {
                    if (this.users[i].Username == username && this.users[i].SessionKey == sessionkey)
                    {
                        legitSessionKey = true;
                        break;
                    }
                }

                if (legitSessionKey == true)
                {
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        if (this.users[i].Username == username)
                        {
                            this.AddLineToLog(username + " (" + ((IPEndPoint)this.users[i].NetworkWatcher.Client.Client.RemoteEndPoint).Address.ToString() + ")" + " logged out!");
                            this.users[i].NetworkWatcher.Stop();
                            this.users.Remove(this.users[i]);
                            break;
                        }
                    }
                }

                // Sends all users the user who logged out
                for (int i = 0; i < this.users.Count; i++)
                {
                    this.users[i].NetworkWatcher.Send(ProtocolCreator.RemoveUser(username));
                    this.users[i].NetworkWatcher.Send(ProtocolCreator.NewMessage("SERVER: " + username + " logged out!"));
                }
            }
        }

        private void AddUser(string username, NetworkWatcher networkWatcher)
        {
            lock (usersLocker)
            {
                if (CheckIfLegalUsername(username) == true)
                {
                    // Sends session key to client
                    Protocol protocol = ProtocolCreator.SessionKey();
                    this.CreateNewUser(username, Encoding.UTF8.GetString(protocol.Content), networkWatcher);

                    networkWatcher.Send(protocol);
                }
            }
        }

        private void FinishUserLogin(string username, string sessionkey)
        {
            lock (usersLocker)
            {
                NetworkWatcher networkWatcher = null;

                // Gets the networkWatcher of the new user
                for (int i = 0; i < this.users.Count; i++)
                {
                    if (this.users[i].Username == username && this.users[i].SessionKey == sessionkey)
                    {
                        networkWatcher = this.users[i].NetworkWatcher;
                    }
                }

                if (networkWatcher != null)
                {
                    // Sends the new user all online users
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        if (this.users[i].Username != username)
                        {
                            networkWatcher.Send(ProtocolCreator.AddUser(this.users[i].Username));
                        }
                    }

                    // Sends all users the user who logged in
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        this.users[i].NetworkWatcher.Send(ProtocolCreator.AddUser(username));
                        this.users[i].NetworkWatcher.Send(ProtocolCreator.NewMessage("SERVER: " + username + " logged in!"));
                    }

                    this.AddLineToLog(username + " (" + ((IPEndPoint)networkWatcher.Client.Client.RemoteEndPoint).Address.ToString() + ")" + " logged in!");
                }
            }
        }

        private void ForwardMessageToAllUsers(string message)
        {
            lock (usersLocker)
            {
                string[] messageArray = message.Split('-');

                if (messageArray.Length == 3)
                {
                    bool verifiedMessage = false;

                    for (int i = 0; i < this.users.Count; i++)
                    {
                        if (this.users[i].Username == messageArray[0] && this.users[i].SessionKey == messageArray[2])
                        {
                            verifiedMessage = true;
                            break;
                        }
                    }

                    if (verifiedMessage == true)
                    {
                        for (int i = 0; i < this.users.Count; i++)
                        {
                            this.users[i].NetworkWatcher.Send(ProtocolCreator.NewMessage(messageArray[0] + ": " + messageArray[1]));
                        }
                    }
                }
            }
        }

        private void ConnectionLost(object sender, ConnectionLostEventArgs args)
        {
            lock (usersLocker)
            {
                try
                {
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        if (((IPEndPoint)this.users[i].NetworkWatcher.Client.Client.RemoteEndPoint).Address == ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address)
                        {
                            if (((IPEndPoint)this.users[i].NetworkWatcher.Client.Client.RemoteEndPoint).Port == ((IPEndPoint)args.Client.Client.RemoteEndPoint).Port)
                            {
                                this.RemoveUser(this.users[i].Username, this.users[i].SessionKey);
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
}
