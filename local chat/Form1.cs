using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace local_chat
{
    public partial class Form1 : Form
    {
        const string MulticastAddress = "224.0.0.224";
        const int Port = 5000;
        const int HelloInterval = 5000;
        const string MessagePrefix = "MSG:";
        const string HelooPrefix = "HELLO:";
        private UdpClient udpClient;
        private IPEndPoint groupEP;
        private Thread receiveThread;
        private bool isRunning;
        private readonly string userName = Environment.MachineName;
        
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
                txtchat.AppendText($"Ошибка отправки:{ex.Message}\r\n");
            }
        }
        private void ProcessReceivedMessage(string message,IPAddress senderIp)
        {
            if (message.StartsWith(HelooPrefix))
            {
                string name = message.Substring(HelooPrefix.Length);
                if (!listUsers.Items.Contains(name)
                   && name != userName)
                {
                    listUsers.Items.Add(name);
                    txtchat.AppendText($"[{name} присоединился]\r\n");
                }
            }
            else if (message.StartsWith(MessagePrefix));
            {
                string text = message.Substring (MessagePrefix.Length);
                txtchat.AppendText($"{text}\r\n");
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
                catch (SocketException) { }



                
            })
            { IsBackground = true };
            receiveThread.Start();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtmsg.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                SendMessage($"{MessagePrefix}{userName}:{text}");
                txtmsg.Clear();
            }
        }
    }
}
