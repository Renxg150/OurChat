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

namespace OurChat
{
    /// <summary>
    /// WindowAllChat.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAllChat : Window
    {
        private Socket? _socket = null;
        public static ObservableCollection<AllMessage> allMessagesList { get; set; }
        public WindowAllChat()
        {
            InitializeComponent();
        }

        public WindowAllChat(Socket _socket)
        {
            this._socket = _socket;
            InitializeComponent();
            allMessagesList = new ObservableCollection<AllMessage>(); // 初始化allMessagesList
            contextSpace.ItemsSource = allMessagesList; // 指定contextSpace的ItemsSource为allMessagesList
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string message = inputField.Text;  // 得到输入框的内容
            if (string.IsNullOrEmpty(message))  // 如果输入框的内容为空的话不作操作
            {
                return;
            }
            else
            {
                this._socket.Send(Encoding.UTF8.GetBytes(message + "<AllMessage>"));  // 发送输入框的内容
                AddMessage("我",message);
                inputField.Clear();
            }
        }

        public static void  AddMessage(string name,string message)
        {
            
            SynchronizationContext.SetSynchronizationContext(new
                                DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
            SynchronizationContext.Current.Post(pl =>
            {
                //里面写真正的业务内容
                // 将数据添加到 allMessagesList 中
                allMessagesList.Add(new AllMessage(message, name, DateTime.Now.ToString()));
            }, null);
        }

    }

}
