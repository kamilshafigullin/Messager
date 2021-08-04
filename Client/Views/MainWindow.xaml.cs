using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace ClientServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void serverStartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 6000);
                MessageBox.Show("Server started");
                serverSocket.Start();
                TcpClient clientSocket = serverSocket.AcceptTcpClient();
                NetworkStream stream = clientSocket.GetStream();
                string message = "hello!";
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }   
        }
    }
}
