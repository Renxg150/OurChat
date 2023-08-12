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

namespace OurChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string _ip = "101.35.44.111";  // 部署到云服务上时运行
        private string _ip = "127.0.0.1";  // 在本地测试运行
        private int _port = 8080;
        private Socket? _socket = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            connectButton.IsEnabled = false;
            connectButton.Content = "正在链接服务器，请等待...";

            // 初始化套接字(IP4寻址地址,流式传输,TCP协议)
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 创建IP对象
            IPAddress address = IPAddress.Parse(_ip);
            // 创建网络端口包括ip和端口
            IPEndPoint endPoint = new IPEndPoint(address, _port);
            // 建立连接
            try
            {
                _socket.Connect(endPoint);
                MessageBox.Show("链接服务器成功！");
                connectButton.Content = "已成功连接至服务器,输入昵称进入";
                label1.Visibility = Visibility.Visible;
                textBox1.Visibility = Visibility.Visible;
                enterButton.Visibility = Visibility.Visible;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connectButton.Content = "连接失败！" + ex.Message;
            }
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            // 得到输入的名字传给下一个窗口
            string name = textBox1.Text;
            // 呼出新窗口
            new Window1(_socket, name).Show();

            // 将原窗口隐藏掉
            this.Visibility = Visibility.Collapsed;
        }
    }
}
