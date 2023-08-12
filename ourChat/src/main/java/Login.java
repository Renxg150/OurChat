import java.nio.channels.SocketChannel;
import java.util.ArrayList;
import java.util.Iterator;

public class Login {

    // 初始化一个 ArrayList 用来存放在线的用户
    ArrayList<User> onlineUsers = new ArrayList<>();

    // 有登录的用户了，就将此用户加入到在线的列表中
    // 如果说已经有了
    public boolean addUser(User user){
        for (User u: onlineUsers) {
            if(u.name.equals(user.name)){
                return false;
            }
        }
        onlineUsers.add(user);
        return true;
    }

    // 将此用户从在线的列表中删除
    public void removeUser(User user){
        onlineUsers.remove(user);
    }

    // 将此用户从在线的列表中删除
    public void removeUser(SocketChannel socketChannel){
        onlineUsers.removeIf(user -> user.socketChannel == socketChannel);
    }

    //
    public String getAllUser(){
        StringBuilder stringBuilder = new StringBuilder();
        for (User u: onlineUsers) {
            stringBuilder.append(u.name);
            stringBuilder.append(',');
        }
        String s = stringBuilder.toString();
        return s.substring(0,s.length()-1);
    }

}
