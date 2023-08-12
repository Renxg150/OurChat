using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using OurChat;

namespace OurChat
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        private Socket? _socket = null;

        // 在线的用户名字列表
        public static ObservableCollection<string>? UserNameList { get; set; }
        // 一个委托
        public delegate void AddMessageFunction(string name, string message);
        // 声明 adFunction 委托
        public static AddMessageFunction adFunction { get; set; }
        public Window1()
        {
            InitializeComponent();
        }
        public Window1(Socket _socket,string name)
        {   
            // 初始化窗口时将设置的昵称信息发送给服务器
            this._socket = _socket;
            this._socket.Send(Encoding.UTF8.GetBytes(name + "<LoginMessage>"));
            InitializeComponent();
            UserNameList = new ObservableCollection<string>();
            // 开启一个线程，用来接收服务器发来的数据
            Thread receiveThread = new Thread(() => ReceiveMessage2(_socket));
            receiveThread.Start();
            //onlineList.ItemsSource = UserNameList;
            // 将添加群聊消息的函数绑定给adFunction委托
            adFunction += WindowAllChat.AddMessage;
        }

        public static void ReceiveMessage(Socket socket, ListBox onlineList)
        {
            // 死循环，一直接收
            while (true)
            {
                // 设置缓冲区来接收服务器发来的信息
                byte[] buffer = new byte[1024];
                
                // 阻塞接收服务器发来的信息
                int len = socket.Receive(buffer); // 阻塞接收
                if (len > 0)
                {
                    // 将接收到的信息转化为字符串
                    string receivedMessage = Encoding.UTF8.GetString(buffer);
                    receivedMessage = receivedMessage.TrimEnd('\0');
                    // 接收用户列表信息
                    if (receivedMessage.EndsWith("<userList>"))
                    {
                        Console.WriteLine(receivedMessage);
                        
                        receivedMessage = receivedMessage.Substring(0, receivedMessage.Length - 10);
                        Console.WriteLine(receivedMessage);
                        // 接收到的用户列表信息
                        //receivedMessage = "2345,789,789456";
                        string[] nameList = receivedMessage.Split(',');
                        Console.WriteLine("nameList:" + nameList);

                        // 先删除所有的元素
                        onlineList.Dispatcher.Invoke((Action)delegate ()
                        {
                            while (onlineList.Items.Count > 0)
                            {
                                onlineList.Items.RemoveAt(0);
                            }
                        });

                        foreach (string name in nameList)
                        {
                            // 在此线程中操作 UI 线程的变量需要调用 Dispatcher ，并且需要使用委托
                            onlineList.Dispatcher.Invoke((Action)delegate ()
                                {
                                    onlineList.Items.Add(name);
                                });
                        }
                    }
                }
            }
        }

        public static void ReceiveMessage2(Socket socket)
        {
            
            // 死循环，一直接收
            while (true)
            {
                // 设置缓冲区来接收服务器发来的信息
                byte[] buffer = new byte[1024];

                // 阻塞接收服务器发来的信息
                int len = socket.Receive(buffer); // 阻塞接收
                if (len > 0)
                {
                    // 将接收到的信息转化为字符串
                    string receivedMessage = Encoding.UTF8.GetString(buffer);
                    receivedMessage = receivedMessage.TrimEnd('\0');
                    // 接收用户列表信息
                    if (receivedMessage.EndsWith("<userList>"))
                    {
                        Console.WriteLine(receivedMessage);

                        receivedMessage = receivedMessage.Substring(0, receivedMessage.Length - 10);
                        Console.WriteLine(receivedMessage);
                        // 接收到的用户列表信息
                        //receivedMessage = "2345,789,789456";
                        string[] nameList = receivedMessage.Split(',');
                        Console.WriteLine("nameList:" + nameList);
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            SynchronizationContext.SetSynchronizationContext(new
                                DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                            SynchronizationContext.Current.Post(pl =>
                            {
                                //里面写真正的业务内容
                                UserNameList.Clear();
                                foreach (string item in nameList)
                                {
                                    UserNameList.Add(item);
                                }
                            }, null);
                        });
                    } else if(receivedMessage.EndsWith("<allMesg>")) // 接受到群聊消息
                    {
                        
                        string tmp = receivedMessage.Substring(0, receivedMessage.Length - 9);
                        // 得到姓名和消息
                        string[] nameAndMessage = tmp.Split("<name>");
                        string message = nameAndMessage[1];
                        string name = nameAndMessage[0];
                        // MessageBox.Show(message + name);
                        // 将姓名和消息传递给群聊窗口
                        adFunction(name,message);
                    }
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _socket.Send(Encoding.UTF8.GetBytes("<getUserList>"));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new WindowAllChat(_socket).Show();
        }
    }
}
