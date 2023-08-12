import java.nio.channels.SocketChannel;

public class User {

    String name;
    SocketChannel socketChannel;

    User(String name,SocketChannel socketChannel){
        this.name = name;
        this.socketChannel = socketChannel;
    }

    @Override
    public String toString() {
        return "User{" +
                "name='" + name + '\'' +
                ", socketChannel=" + socketChannel +
                '}';
    }
}
