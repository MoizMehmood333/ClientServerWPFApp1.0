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

namespace ServerChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        Socket mainSock;
        Socket accSock;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        Socket socket() {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            mainSock = socket();
            mainSock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000));
            mainSock.Listen(0);
            new Thread(() =>
            {
                accSock = mainSock.Accept();
                MessageBox.Show("Connection Accepted");
                mainSock.Close();

                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[255];
                        int receive = accSock.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        if (receive <= 0)
                        {
                            throw new SocketException();
                        }

                        Array.Resize(ref buffer, receive);

                        Application.Current.Dispatcher.Invoke(() => {
                            txtResponse.Text = Encoding.UTF8.GetString(buffer);
                        });
                    }
                    catch 
                    {

                        MessageBox.Show("Disconnection");
                        accSock.Close();
                        break;
                    }
                  
                
                }
            }).Start();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(txtSend.Text);
            accSock.Send(data, 0, data.Length, SocketFlags.None);
        }
    }
}
