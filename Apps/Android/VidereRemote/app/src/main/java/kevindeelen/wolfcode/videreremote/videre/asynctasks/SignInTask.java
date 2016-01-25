package kevindeelen.wolfcode.videreremote.videre.asynctasks;

import android.os.AsyncTask;

import java.io.IOException;

/**
 * Created by Wolf on 25-Jan-16.
 */
public class SignInTask extends AsyncTask<SignInTaskArgument,Void,Boolean>
{

    @Override
    protected Boolean doInBackground ( SignInTaskArgument... params )
    {
        try
        {
            params[ 0 ].client.Connect( params[ 0 ].ip, params[ 0 ].port );
            return true;
        } catch ( IOException e )
        {
            e.printStackTrace( );
            return false;
        }
    }
}
