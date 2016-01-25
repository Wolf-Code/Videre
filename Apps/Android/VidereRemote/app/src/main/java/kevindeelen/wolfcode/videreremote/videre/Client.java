package kevindeelen.wolfcode.videreremote.videre;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

/**
 * Created by Wolf on 23-Jan-16.
 */
public class Client
{
    private Socket client;
    private InputStream input;
    private OutputStream output;

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
        this.input = client.getInputStream( );
        this.output = client.getOutputStream( );
    }

    public void sendData ( byte[] data ) throws IOException
    {
        this.output.write( data );
    }

    public byte[] readData ( int byteCount ) throws IOException
    {
        byte[] buffer = new byte[ byteCount ];
        this.input.read( buffer );

        return buffer;
    }

    public void Close ( ) throws IOException
    {
        this.client.close( );
    }
}
