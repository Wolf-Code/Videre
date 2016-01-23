package kevindeelen.wolfcode.videreremote.activity;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import kevindeelen.wolfcode.videreremote.R;


public class ConnectorFragment extends Fragment
{

    public ConnectorFragment ( )
    {
        // Required empty public constructor
    }

    @Override
    public void onCreate ( Bundle savedInstanceState )
    {
        super.onCreate( savedInstanceState );
    }

    @Override
    public View onCreateView ( LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState )
    {
        View rootView = inflater.inflate( R.layout.fragment_connector, container, false );

        // Inflate the layout for this fragment
        return rootView;
    }

    @Override
    public void onDetach ( )
    {
        super.onDetach( );
    }
}