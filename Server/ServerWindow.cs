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
        private static object locker;

        public ServerWindow()
        {
            InitializeComponent();
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
                        locker = new object();
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
                this.listener.Stop();
                this.isRunning = false;
                this.ipAddressPubTextBox.Text = string.Empty;
                this.ipAddressPrivTextBox.Text = string.Empty;
                this.portTextBox.Enabled = true;
                this.AddLineToLog("The server was successfully stopped!");
            }
        }

        private bool CheckIfLegalUsername(string username)
        {
            lock (locker)
            {
                if (username.Length >= 3 && username.Length <= 10 && username != "SERVER")
                {
                    foreach (User user in this.users)
                    {
                        if (user.Username == username)
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
            lock (locker)
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
                    string username = Encoding.ASCII.GetString(args.Protocol.Content);
                    this.AddUser(username, (NetworkWatcher)sender);
                }
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.LogOut))
            {
                if (args.Protocol.Content != null && args.Protocol.Content.Length >= 1)
                {
                    string usernameAndSessionKey = Encoding.ASCII.GetString(args.Protocol.Content);

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
                    string message = Encoding.ASCII.GetString(args.Protocol.Content);
                    this.ForwardMessageToAllUsers(message);
                }
            }
        }

        private void RemoveUser(string username, string sessionkey)
        {
            lock (locker)
            {
                // Verifys session key of the logout request
                bool legitSessionKey = false;

                foreach (User user in this.users)
                {
                    if (user.Username == username && user.SessionKey == sessionkey)
                    {
                        legitSessionKey = true;
                        break;
                    }
                }

                if (legitSessionKey == true)
                {
                    // Remove user from user list on server
                    foreach (User user in this.users)
                    {
                        if (user.Username == username)
                        {
                            this.AddLineToLog(username + " (" + ((IPEndPoint)user.NetworkWatcher.Client.Client.RemoteEndPoint).Address.ToString() + ")" + " logged out!");
                            user.NetworkWatcher.Stop();
                            this.users.Remove(user);
                            break;
                        }
                    }
                }

                // Sends all users the user who logged out
                foreach (User user in this.users)
                {
                    user.NetworkWatcher.Send(ProtocolCreator.RemoveUser(username));
                    user.NetworkWatcher.Send(ProtocolCreator.NewMessage("SERVER: " + username + " logged out!"));
                }
            }
        }

        private void AddUser(string username, NetworkWatcher networkWatcher)
        {
            lock (locker)
            {
                if (CheckIfLegalUsername(username) == true)
                {
                    Protocol protocol = ProtocolCreator.SessionKey();
                    this.CreateNewUser(username, Encoding.ASCII.GetString(protocol.Content), networkWatcher);

                    networkWatcher.Send(protocol);

                    Thread.Sleep(100);

                    // Sends the new user all online users
                    foreach (User user in this.users)
                    {
                        if (user.Username != username)
                        {
                            networkWatcher.Send(ProtocolCreator.AddUser(user.Username));
                        }
                    }

                    // Sends all users the new user
                    foreach (User user in this.users)
                    {
                        user.NetworkWatcher.Send(ProtocolCreator.AddUser(username));
                        user.NetworkWatcher.Send(ProtocolCreator.NewMessage("SERVER: " + username + " logged in!"));
                    }

                    this.AddLineToLog(username + " (" + ((IPEndPoint)networkWatcher.Client.Client.RemoteEndPoint).Address.ToString() + ")" + " logged in!");
                }
            }
        }

        private void ForwardMessageToAllUsers(string message)
        {
            lock (locker)
            {
                string[] messageArray = message.Split('-');

                if (messageArray.Length == 3)
                {
                    bool verifiedMessage = false;

                    foreach (User user in this.users)
                    {
                        if (user.Username == messageArray[0] && user.SessionKey == messageArray[2])
                        {
                            verifiedMessage = true;
                            break;
                        }
                    }

                    if (verifiedMessage == true)
                    {
                        foreach (User user in this.users)
                        {
                            user.NetworkWatcher.Send(ProtocolCreator.NewMessage(messageArray[0] + ": " + messageArray[1]));
                        }
                    }
                }
            }
        }

        private void ConnectionLost(object sender, ConnectionLostEventArgs args)
        {
            lock (locker)
            {
                try
                {
                    foreach (User user in this.users)
                    {
                        if (((IPEndPoint)user.NetworkWatcher.Client.Client.RemoteEndPoint).Address == ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address)
                        {
                            if (((IPEndPoint)user.NetworkWatcher.Client.Client.RemoteEndPoint).Port == ((IPEndPoint)args.Client.Client.RemoteEndPoint).Port)
                            {
                                this.RemoveUser(user.Username, user.SessionKey);
                                break;
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}
