using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket sock;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sock.Close();
        }

        Socket socket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }
        void Read() {
            while (true) {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receive = sock.Receive(buffer);
                    if (receive <= 0) {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, receive);

                    Application.Current.Dispatcher.Invoke(() => {
                        txtResponse.Text = Encoding.ASCII.GetString(buffer);
                    });

                }
                catch 
                {

                    MessageBox.Show("Server DISCONNECTED");
                    sock.Close();
                    break;
                }
            }
        }

        private void btnConnext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sock = socket();
                sock.Connect(new IPEndPoint(IPAddress.Parse(txtIPAddress.Text), 4000));
                new Thread(() => {
                    Read();
                }).Start();
                    
            }
            catch {
                MessageBox.Show("CONNECTION FAILED!");
            }


        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(txtSend.Text);
            sock.Send(data, 0, data.Length, SocketFlags.None) ;
        }
    }
}
