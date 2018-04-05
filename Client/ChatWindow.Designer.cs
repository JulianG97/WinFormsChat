using System;

namespace Client
{
    partial class ChatWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatWindow));
            this.messageRichTextBox = new System.Windows.Forms.RichTextBox();
            this.onlineUserRichTextBox = new System.Windows.Forms.RichTextBox();
            this.messagesLabel = new System.Windows.Forms.Label();
            this.onlineUsersLabel = new System.Windows.Forms.Label();
            this.sendMessageTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // messageRichTextBox
            // 
            this.messageRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.messageRichTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.messageRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageRichTextBox.Location = new System.Drawing.Point(26, 52);
            this.messageRichTextBox.Name = "messageRichTextBox";
            this.messageRichTextBox.ReadOnly = true;
            this.messageRichTextBox.Size = new System.Drawing.Size(852, 528);
            this.messageRichTextBox.TabIndex = 0;
            this.messageRichTextBox.TabStop = false;
            this.messageRichTextBox.Text = "";
            // 
            // onlineUserRichTextBox
            // 
            this.onlineUserRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.onlineUserRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlineUserRichTextBox.Location = new System.Drawing.Point(907, 52);
            this.onlineUserRichTextBox.Name = "onlineUserRichTextBox";
            this.onlineUserRichTextBox.ReadOnly = true;
            this.onlineUserRichTextBox.Size = new System.Drawing.Size(227, 528);
            this.onlineUserRichTextBox.TabIndex = 1;
            this.onlineUserRichTextBox.TabStop = false;
            this.onlineUserRichTextBox.Text = "";
            // 
            // messagesLabel
            // 
            this.messagesLabel.AutoSize = true;
            this.messagesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messagesLabel.Location = new System.Drawing.Point(19, 1);
            this.messagesLabel.Name = "messagesLabel";
            this.messagesLabel.Size = new System.Drawing.Size(190, 42);
            this.messagesLabel.TabIndex = 2;
            this.messagesLabel.Text = "Messages";
            // 
            // onlineUsersLabel
            // 
            this.onlineUsersLabel.AutoSize = true;
            this.onlineUsersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlineUsersLabel.Location = new System.Drawing.Point(900, 1);
            this.onlineUsersLabel.Name = "onlineUsersLabel";
            this.onlineUsersLabel.Size = new System.Drawing.Size(215, 42);
            this.onlineUsersLabel.TabIndex = 3;
            this.onlineUsersLabel.Text = "Online User";
            // 
            // sendMessageTextBox
            // 
            this.sendMessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendMessageTextBox.Location = new System.Drawing.Point(26, 635);
            this.sendMessageTextBox.Name = "sendMessageTextBox";
            this.sendMessageTextBox.Size = new System.Drawing.Size(852, 44);
            this.sendMessageTextBox.TabIndex = 4;
            // 
            // sendButton
            // 
            this.sendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.Location = new System.Drawing.Point(907, 633);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(239, 54);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(19, 588);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(200, 42);
            this.usernameLabel.TabIndex = 6;
            this.usernameLabel.Text = "Username:";
            // 
            // ChatWindow
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 700);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.sendMessageTextBox);
            this.Controls.Add(this.onlineUsersLabel);
            this.Controls.Add(this.messagesLabel);
            this.Controls.Add(this.onlineUserRichTextBox);
            this.Controls.Add(this.messageRichTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ChatWindow";
            this.Text = "Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox messageRichTextBox;
        private System.Windows.Forms.RichTextBox onlineUserRichTextBox;
        private System.Windows.Forms.Label messagesLabel;
        private System.Windows.Forms.Label onlineUsersLabel;
        private System.Windows.Forms.TextBox sendMessageTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label usernameLabel;
    }
}