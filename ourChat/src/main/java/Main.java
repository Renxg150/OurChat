import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.*;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.Set;

public class Main {

    public static void main(String[] args) throws IOException
    {
        Login loginRecord = new Login();  // 用来记录在线用户
        // 开启一个 serverSocketChannel
        ServerSocketChannel serverSocketChannel = ServerSocketChannel.open();
        // 设置非阻塞模式
        serverSocketChannel.configureBlocking(false);
        // 绑定监听
        serverSocketChannel.socket().bind(new InetSocketAddress(8080));
        // 创建 Selector
        Selector selector = Selector.open();
        // 将 ServerSocketChannel 注册到 Selector 上，监听SelectionKey.ACCEPT
        serverSocketChannel.register(selector, SelectionKey.OP_ACCEPT);
        System.out.println("服务器已启动...");
        // select()阻塞到至少有一个通道在你注册的事件上就绪了
        // 如果没有准备好的channel,就在这一直阻塞
        // select(long timeout)和select()一样,除了最长会阻塞timeout毫秒(参数)。
        while(selector.select() > 0)
        {
            // 选择准备好的key
            Set<SelectionKey> selectionKeys = selector.selectedKeys();
            // 得到迭代器
            Iterator<SelectionKey> iterator = selectionKeys.iterator();
            while (iterator.hasNext())
            {
                // 拿到 SelectionKey
                SelectionKey next = iterator.next();
                // 移除SelectionKey，因为 selectedKeys 不会自己移除
                iterator.remove();
                if(next.isAcceptable())
                {
                    // 得到准备好的 ServerSocketChannel 对象
                    ServerSocketChannel channel = (ServerSocketChannel)next.channel();
                    // 得到 SocketChannel
                    SocketChannel accept = channel.accept();
                    // 设置 SocketChannel 非阻塞
                    accept.configureBlocking(false);
                    // 向selector注册 SocketChannel 的读消息
                    accept.register(selector,SelectionKey.OP_READ);
                    System.out.println("有一个客户端加入链接，SocketChannel为"+accept);
                }
                else if (next.isReadable() && next.isValid())
                {
                    // 得到准备好的 SocketChannel 对象
                    SocketChannel channel = (SocketChannel) next.channel();
                    // 临时使用的 byteBuffer 接收消息
                    ByteBuffer byteBuffer = ByteBuffer.allocate(1024);
                    // 将消息读取到 byteBuffer中
                    try
                    {
                        int read = channel.read(byteBuffer);
                        if(read == -1)
                        {
                            // 取消当前key
                            next.cancel();
                            // 关闭socket
                            channel.socket().close();
                            // 关闭 channel
                            channel.close();
                            System.out.println("read = -1 ,"+ channel + "已退出");
                            // 将用户踢出去
                            loginRecord.removeUser(channel);
                        } else
                        {
                            // 将 byteBuffer 切换到读模式
                            byteBuffer.flip();
                            String message = StandardCharsets.UTF_8.decode(byteBuffer).toString();
                            // 查看发送的是不是昵称
                            if(message.endsWith("<LoginMessage>"))
                            {
                                // 处理登录的信息
                                dealWithLoginMessage(loginRecord, channel, message);
                            }
                            else if(message.endsWith("<getUserList>"))
                            {  // 将用户列表发送给客户端
                                System.out.println("8217398" + message);
                                // 将在线的用户列表以字符串的方式返回给客户端
                                String allUser = loginRecord.getAllUser();
                                channel.write(ByteBuffer.wrap((allUser + "<userList>").getBytes(StandardCharsets.UTF_8)));
                            }
                            else if (message.endsWith("<AllMessage>"))  // 群聊消息
                            {

                                String allMessage = message.substring(0,message.length() - 12);
                                String sender = "";
                                for(User u : loginRecord.onlineUsers) // 查找发送者的昵称
                                {
                                    if (u.socketChannel == channel) // 发送者
                                    {
                                        sender = u.name;  // 得到昵称
                                    }
                                }
                                System.out.println("发送群聊消息" + sender + "message:" + allMessage);
                                for(User u : loginRecord.onlineUsers)  // 将消息发送给除了发送者的所有人
                                {
                                    if(u.socketChannel == channel) // 跳过发送者
                                    {
                                        continue;
                                    }
                                    // 逐一发送消息
                                    u.socketChannel.write(ByteBuffer.wrap((sender + "<name>" + allMessage + "<allMesg>").getBytes(StandardCharsets.UTF_8)));
                                }
                            }
//                            System.out.println("----");
                            // 打印消息查看
                            System.out.println(message);
                        }
                    } catch (IOException e)
                    { // 客户端退出将会发生此异常
                        // 取消当前key
                        next.cancel();
                        // 将用户踢出去
                        loginRecord.removeUser(channel);
                        // 关闭socket
                        channel.socket().close();
                        // 关闭 channel
                        channel.close();
                        e.printStackTrace();
                    }
                }
            }
        }
    }

    // 处理登录信息的方法
    private static void dealWithLoginMessage(Login loginRecord, SocketChannel channel, String message) throws IOException {
        String substring = message.substring(0, message.length() - 14);
        User user = new User(substring, channel);
        boolean b = loginRecord.addUser(user);
        if(b){
            System.out.println("登录成功！" + user);
            // 将在线的用户列表以字符串的方式返回给客户端
            String allUser = loginRecord.getAllUser();

            channel.write(ByteBuffer.wrap((allUser + "<userList>").getBytes(StandardCharsets.UTF_8)));
            System.out.println("已经向客户端写消息" + allUser);
        } else {
            System.out.println("登录失败！" + user);
        }
    }
}