package kevindeelen.wolfcode.videreremote.videre;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;

/**
 * Created by Wolf on 23-Jan-16.
 */
public class Client
{
    private Socket client;

    public Client ( )
    {

    }

    public Socket getSocket ( )
    {
        return client;
    }

    public void Connect ( String ip, int port ) throws IOException
    {
        InetAddress serverIP = InetAddress.getByName( ip );
        client = new Socket( serverIP, port );
    }
}
