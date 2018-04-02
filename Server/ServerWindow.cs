﻿using System;
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
                        this.listener = new TcpListener(IPAddress.Any, port);
                        this.listener.Start();

                        this.isRunning = true;
                        this.portTextBox.Enabled = false;
                        this.AddLineToLog("The server was successfully started!");

                        StartListening();
                    }
                    catch
                    {
                        this.isRunning = false;
                        this.portTextBox.Enabled = true;
                        this.AddLineToLog("The server was successfully stopped!");

                        MessageBox.Show("The server couldn't be started!", "Chat - Server", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
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

        private void StartListening()
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
                this.isRunning = false;
                this.portTextBox.Enabled = true;
                this.AddLineToLog("The server was successfully stopped!");
            }
        }
    }
}
