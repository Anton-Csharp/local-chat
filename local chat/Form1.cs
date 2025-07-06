using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace local_chat
{
    public partial class Form1 : Form
    {
        const string MulticastAddress = "224.0.0.224";
        const int Port = 5000;
        const int HelloInterval = 5000;
        const string MessagePrefix = "MSG:";
        const string HelooPrefix = "HELLO:";
        const string privatePrefix = "PRIVATE:";
        private UdpClient udpClient;
        private IPEndPoint groupEP;
        private Thread receiveThread;
        private bool isRunning;
        private string userName = Environment.MachineName;
        private readonly Dictionary <string,IPAddress > users = new Dictionary<string, IPAddress> ();
        
        
        public Form1()
        {
            InitializeComponent();
            InitializeNetwork();
            SetupUI();
            Startlistening();
        }
        private void InitializeNetwork()
        {
            try
            {
                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
                );
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                udpClient.JoinMulticastGroup(IPAddress.Parse(MulticastAddress));
                groupEP = new IPEndPoint(IPAddress.Parse(MulticastAddress),Port );
                isRunning = true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"ошибка{ex.Message}");
                Close();
            }
        }
        private void SetupUI()
        {
            Text = $"локальный чат:{userName}";
            FormClosing += Form1_FormClosing;
            btnSend.Click += btnSend_Click;
            var timer  =  new System.Windows.Forms.Timer { Interval = HelloInterval};
            timer.Tick += (s, e) => SendMessage(HelooPrefix + userName);
            timer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false ;
            udpClient?.Close();
            receiveThread ?.Join(1000);
        }
       
        private void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, groupEP);
            }
            catch(Exception ex)
            {
                //txtchat.AppendText($"Ошибка отправки:{ex.Message}\r\n");
                AddUserDataInChat("система", ex.Message);
            }
        }
        private void ProcessReceivedMessage(string message,IPAddress senderIp)
        {
            if (message.StartsWith(HelooPrefix))
            {
                string name = message.Substring(HelooPrefix.Length);
                if (name != userName )
                {
                    users[name] = senderIp;
                    Updateuserlist();
                    if (!listUsers.Items.Contains(name))
                    {
                        //txtchat.AppendText($"[{name} присоединился]\r\n");
                        AddUserDataInChat(name, "новый пользователь!");
                    }
                }
            }
            else if (message.StartsWith(MessagePrefix))
            {
                string content = message.Substring(MessagePrefix.Length);

                int nameEndIndex = content.IndexOf(':');
                if (nameEndIndex < 0) return;

                string name = content.Substring(0,nameEndIndex);
                if (name == userName) return;

                string rest = content.Substring(nameEndIndex+1);

                int avatarEndIndex = rest.IndexOf(':');
                if (avatarEndIndex < 0) return;

                string avatarBase64 = rest.Substring(0, avatarEndIndex);
                string msg= rest.Substring(avatarEndIndex+1);

                Image avatar = null;
                if (!string.IsNullOrEmpty(msg))
                {
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(avatarBase64);
                        using(MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            avatar = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    AddUserDataInChat(name,msg,avatar );
                }
            }
            else if (message.StartsWith(privatePrefix))
            {
                string content = message.Substring(privatePrefix.Length);
                int separatorindex = content.IndexOf(':');
                if (separatorindex >0)
                {
                    string senderName = content.Substring(0, separatorindex);
                    string privatemessage = content.Substring(separatorindex + 1);
                    if (senderName == userName || users.ContainsKey(senderName))
                    {
                        //txtchat.AppendText($"[ЛИЧНО от {senderName}]:{privatemessage}\r\n");
                        AddUserDataInChat(senderName, privatemessage);   
                    }
                }
            }

        }
        private void Startlistening()
        {
            receiveThread = new Thread(() =>
            {
                try
                {
                    while (isRunning)
                    {
                        IPEndPoint remoteEP = null;
                        byte[] data = udpClient.Receive(ref remoteEP);
                        string message = Encoding.UTF8.GetString(data);
                        this.Invoke((MethodInvoker)delegate{  
                             ProcessReceivedMessage(message, remoteEP.Address);
                        });

                    }
                }
                catch (SocketException)
                { 

                }



                
            })
            { IsBackground = true };
            receiveThread.Start();
           
        }
          
        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtmsg.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                string avatarbase64 = "";
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        avatarbase64 = Convert.ToBase64String(ms.ToArray());
                    }
                    SendMessage($"{MessagePrefix}{userName}:{avatarbase64}:{text}");
                    AddUserDataInChat(userName, text, pictureBox1.Image);
                    txtmsg.Clear();
                }
                
            }
        }

        private void btnSendPrivateMsg_Click(object sender, EventArgs e)
        {
            if (listUsers.SelectedItem == null)
            {
                MessageBox.Show("выберите пользавателя из списка");
                return;
            }
            string recipient = listUsers.SelectedItem.ToString();
            string privatetext = txtmsg.Text.Trim();
            if (!string.IsNullOrEmpty(privatetext))
            {
                SendPrivateMessage(recipient, privatetext);
            }
        }
        private void Updateuserlist()
        {
            listUsers.Items.Clear();
            foreach (var user in users.Keys )
            {
                listUsers.Items.Add(user);
            }
        }

        private void SendPrivateMessage(string recipient,string message)
        {
            if (users.TryGetValue(recipient, out IPAddress recipientIp))
            {
                try
                {
                    string fullmessage = $"{privatePrefix}{recipient}:{message}";
                    byte[] data = Encoding.UTF8.GetBytes(fullmessage);
                    IPEndPoint privateEP = new IPEndPoint(recipientIp, Port);
                    udpClient.Send(data, data.Length, privateEP);
                    //txtchat.AppendText($"[Я->{recipient}]:{message}\r\n");
                    AddUserDataInChat(recipient, message);
                }
                catch(Exception ex)
                {
                    //txtchat.AppendText($"Ошибка отправки сообщения:{ex.Message}\r\n");
                    AddUserDataInChat("система",ex.Message);
                }
            }
            else
            {
                MessageBox.Show("пользователь не найден");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()== DialogResult.OK )
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }
        private void AddUserDataInChat (string name,string message,Image avatar = null)
        {
            msgtext msgtext = new msgtext
            {
                username = name,
                image = avatar,
                usertext = message
            };
            flowchat.Controls.Add(msgtext);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Укажите ник")
            {
                textBox1.ForeColor = Color.Black;
                textBox1.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.ForeColor = Color.Silver;
                textBox1.Text = "Укажите ник";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != ""&&textBox1.Text != "Укажите ник")
            {
                userName = textBox1.Text;
            }
            else
            {
                userName = Environment.MachineName;
            }
        }

       
    }
}
