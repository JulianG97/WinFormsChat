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

namespace Client
{
    public partial class LoginWindow : Form
    {
        private string sessionkey;
        private NetworkWatcher networkWatcher;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (this.CheckIfLegalIPAddress(this.ipAddressTextBox.Text) == false)
            {
                MessageBox.Show("Please enter a legal IPv4 address!", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else if (CheckIfLegalPort(this.portTextBox.Text) == false)
            {
                MessageBox.Show("The port must be between 3000 and 9000!", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else if (this.usernameTextBox.Text.ToCharArray().Length < 3)
            {
                MessageBox.Show("The username must be at least 3 characters long!", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                this.networkWatcher = new NetworkWatcher(IPAddress.Parse(this.ipAddressTextBox.Text), int.Parse(this.portTextBox.Text));
                this.networkWatcher.ConnectionLost += this.ConnectionLost;
                this.networkWatcher.DataReceived += this.DataReceived;
                this.networkWatcher.Start();

                if (this.networkWatcher.Connected == true)
                {
                    this.networkWatcher.Send(ProtocolCreator.LogIn(this.usernameTextBox.Text));

                    if (this.WaitForSessionKey(1000) == true)
                    {
                        this.networkWatcher.ConnectionLost -= this.ConnectionLost;
                        this.networkWatcher.DataReceived -= this.DataReceived;
                        this.Hide();

                        ChatWindow chatWindow = new ChatWindow(this.usernameTextBox.Text, this.sessionkey, this.networkWatcher);
                        chatWindow.FormClosed += this.ChatWindowClosed;
                        chatWindow.Show();
                    }
                }
            }
        }

        private void ChatWindowClosed(object sender, FormClosedEventArgs args)
        {
            this.Close();
        }

        private bool WaitForSessionKey(int milliseconds)
        {
            for (int i = 0; i < milliseconds / 10; i++)
            {
                if (this.sessionkey != null)
                {
                    break;
                }

                Thread.Sleep(10);
            }

            if (this.sessionkey == null)
            {
                this.ConnectionLost(this, new EventArgs());
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            MessageBox.Show("Can't connect to the server! Check the IP address and port of the server or try again later.", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Protocol.Type.SequenceEqual(ProtocolType.SessionKey))
            {
                this.sessionkey = Encoding.UTF8.GetString(args.Protocol.Content);
            }
        }

        private bool CheckIfLegalPort(string portString)
        {
            if (int.TryParse(portString, out int port) == false)
            {
                return false;
            }
            else if (port < 3000 || port > 9000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckIfLegalIPAddress(string ipAddress)
        {
            string[] ipAddressArray = ipAddress.Split('.');

            if (ipAddressArray.Length != 4)
            {
                return false;
            }

            for (int i = 0; i < ipAddressArray.Length; i++)
            {
                if (int.TryParse(ipAddressArray[i], out int number) == true)
                {
                    if (number < 0 || number > 255)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
