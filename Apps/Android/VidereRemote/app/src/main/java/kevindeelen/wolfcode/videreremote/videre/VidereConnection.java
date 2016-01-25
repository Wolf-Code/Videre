package kevindeelen.wolfcode.videreremote.videre;

import java.io.IOException;

import kevindeelen.wolfcode.videreremote.videre.asynctasks.SignInTask;
import kevindeelen.wolfcode.videreremote.videre.asynctasks.SignInTaskArgument;

/**
 * Created by Wolf on 25-Jan-16.
 */
public class VidereConnection
{
    private static Client client;

    public static boolean IsConnected ( )
    {
        return client != null && client.getSocket( ).isConnected( );
    }

    public static void SendData ( byte[] data )
    {
        if ( !IsConnected( ) ) return;

        try
        {
            client.sendData( data );
        } catch ( IOException e )
        {
            try
            {
                client.Close( );
            } catch ( IOException e1 )
            {
                e1.printStackTrace( );
            }
            client = null;
        }
    }

    public static void Connect ( String ip, int port ) throws Exception
    {
        if ( IsConnected( ) )
            throw new Exception( "Attempting to connect whilst already connected." );

        SignInTaskArgument arg = new SignInTaskArgument( );
        client = new Client( );
        arg.client = client;
        arg.ip = ip;
        arg.port = port;

        new SignInTask( ).execute( arg );
    }
}