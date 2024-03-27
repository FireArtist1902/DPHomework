using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DPHomework
{
    public partial class Form1 : Form
    {
        Thread thread;

        List<Quote> quotes = new List<Quote>() { new Quote() {Text = "Text1", Author = "Author1" },
            new Quote() {Text = "Text2", Author="Author2" } };
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (thread != null)
            {
                return;
            }

            Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.IP);
            IPAddress iP = Dns.GetHostAddresses(Dns.GetHostName())[1];
            IPEndPoint point = new IPEndPoint(iP, 11000);
            socket.Bind(point);
            thread = new Thread(ReceiveFunc);
            thread.IsBackground = true;
            thread.Start(socket);
            Text = "Working";
        }

        private void ReceiveFunc(object? obj)
        {
            Socket socket = obj as Socket;
            byte[] buff = new byte[1024];
            EndPoint ep = new IPEndPoint(IPAddress.Any, 11000);
            do
            {
                int len = socket.ReceiveFrom(buff, ref ep);
                StringBuilder builder = new StringBuilder(textBox1.Text);
                builder.AppendLine(Encoding.Unicode.GetString(buff, 0, len));
                textBox1.BeginInvoke(new Action<string>(AddText), builder.ToString());
            } while (true);
        }

        private void AddText(string obj)
        {
            StringBuilder builder = new StringBuilder(textBox1.Text);
            builder.AppendLine(obj);
            textBox1.Text = builder.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName())[1];
            IPEndPoint endPoint = new IPEndPoint(address, 11000);
            Socket sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            sendSocket.SendTo(Encoding.Unicode.GetBytes(quotes[rnd.Next(0, quotes.Count)].ToString()), endPoint);
            sendSocket.Shutdown(SocketShutdown.Both);
            sendSocket.Close();
        }
    }
}