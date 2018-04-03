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

        public ChatWindow(string username, NetworkWatcher networkWatcher)
        {
            InitializeComponent();

            this.username = username;
            this.networkWatcher = networkWatcher;
            this.networkWatcher.ConnectionLost += this.ConnectionLost;
            this.networkWatcher.DataReceived += this.DataReceived;
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Protocol.Type.SequenceEqual(ProtocolType.AddUser))
            {

            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.RemoveUser))
            {

            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.SessionKey))
            {

            }
            else if (args.Protocol.Type.SequenceEqual(ProtocolType.NewMessage))
            {

            }
        }

        private void ConnectionLost(object sender, ConnectionLostEventArgs args)
        {

        }
    }
}
