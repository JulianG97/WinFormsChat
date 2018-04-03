using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatWindow : Form
    {
        private string username;
        private string sessionkey;
        private NetworkWatcher networkWatcher;

        public ChatWindow(string username, string sessionkey, NetworkWatcher networkWatcher)
        {
            InitializeComponent();

            this.username = username;
            this.usernameLabel.Text = username + ":";
            this.sessionkey = sessionkey;
            this.networkWatcher = networkWatcher;
            this.networkWatcher.ConnectionLost += this.ConnectionLost;
            this.networkWatcher.DataReceived += this.DataReceived;
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Protocol.Type.SequenceEqual(ProtocolType.AddUser))
            {
                this.AddUserToOnlineBox(Encoding.ASCII.GetString(args.Protocol.Content));
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.RemoveUser))
            {
                this.RemoveUserFromOnlineBox(Encoding.ASCII.GetString(args.Protocol.Content));
            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.NewMessage))
            {
                this.AddMessageToChat(Encoding.ASCII.GetString(args.Protocol.Content));
            }
        }

        private void AddUserToOnlineBox(string username)
        {
            MethodInvoker methodInvokerDelegate = delegate()
            {
                string[] newOnlineUserBox = new string[this.onlineUserRichTextBox.Lines.Length + 1];

                for (int i = 0; i < this.onlineUserRichTextBox.Lines.Length; i++)
                {
                    newOnlineUserBox[i] = this.onlineUserRichTextBox.Lines[i];
                }

                newOnlineUserBox[this.onlineUserRichTextBox.Lines.Length] = username;

                this.onlineUserRichTextBox.Lines = newOnlineUserBox;
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

        private void RemoveUserFromOnlineBox(string username)
        {
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                string[] newOnlineUserBox = new string[this.onlineUserRichTextBox.Lines.Length - 1];

                for (int i = 0, j = 0; i < this.onlineUserRichTextBox.Lines.Length; i++, j++)
                {
                    if (this.onlineUserRichTextBox.Lines[i] != username)
                    {
                        newOnlineUserBox[j] = this.onlineUserRichTextBox.Lines[i];
                    }
                    else
                    {
                        j--;
                    }
                }

                this.onlineUserRichTextBox.Lines = newOnlineUserBox;
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

        private void AddMessageToChat(string message)
        {
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                string[] newMessageBox = new string[this.messageRichTextBox.Lines.Length + 1];

                for (int i = 0; i < this.messageRichTextBox.Lines.Length; i++)
                {
                    newMessageBox[i] = this.messageRichTextBox.Lines[i];
                }

                newMessageBox[this.messageRichTextBox.Lines.Length] = message;

                this.messageRichTextBox.Lines = newMessageBox;

                if (this.messageRichTextBox.Lines.Length > 1)
                {
                    this.messageRichTextBox.SelectionStart = this.messageRichTextBox.Text.Length - this.messageRichTextBox.Lines[this.messageRichTextBox.Lines.Length - 1].Length - 1;
                    this.messageRichTextBox.ScrollToCaret();
                }
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

        private void ConnectionLost(object sender, EventArgs args)
        {

        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                if (this.sendMessageTextBox.Text.Length >= 1)
                {
                    this.networkWatcher.Send(ProtocolCreator.Message(this.username, this.sendMessageTextBox.Text, this.sessionkey));
                    this.sendMessageTextBox.Text = string.Empty;
                }
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
}
