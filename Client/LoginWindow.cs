using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class LoginWindow : Form
    {
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
                NetworkWatcher networkWatcher = new NetworkWatcher(IPAddress.Parse(this.ipAddressTextBox.Text), int.Parse(this.portTextBox.Text));
                networkWatcher.Start();

                if (networkWatcher.Connected == true)
                {
                    
                }
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
