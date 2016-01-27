package layout;


import android.os.Bundle;
import android.support.design.widget.Snackbar;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.MenuItem.OnMenuItemClickListener;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.NumberPicker;

import java.util.regex.Pattern;

import kevindeelen.wolfcode.videreremote.R;

/**
 * A simple {@link Fragment} subclass.
 */
public class EnterConnectionFragment extends Fragment
{
	private static final Pattern PARTIAl_IP_ADDRESS = Pattern.compile( "^((25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[0-9])\\.){0,3}((25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[0-9])){0,1}$" );

	public EnterConnectionFragment( )
	{
		// Required empty public constructor
	}

	@Override
	public void onCreate( Bundle savedInstanceState )
	{
		super.onCreate( savedInstanceState );

		setHasOptionsMenu( true );
	}

	@Override
	public View onCreateView( LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState )
	{
		// Inflate the layout for this fragment
		View root = inflater.inflate( R.layout.fragment_enter_connection, container, false );

		EditText ipEntry = ( EditText ) root.findViewById( R.id.listenIPEntry );
		ipEntry.addTextChangedListener( new TextWatcher( )
		{
			private String previousText = "";

			@Override
			public void beforeTextChanged( CharSequence s, int start, int count, int after )
			{
			}

			@Override
			public void onTextChanged( CharSequence s, int start, int before, int count )
			{
			}

			@Override
			public void afterTextChanged( Editable s )
			{
				if ( PARTIAl_IP_ADDRESS.matcher( s ).matches( ) ) previousText = s.toString( );
				else s.replace( 0, s.length( ), previousText );
			}
		} );
		NumberPicker portEntry = ( NumberPicker ) root.findViewById( R.id.listenPortEntry );
		portEntry.setMinValue( 1024 );
		portEntry.setMaxValue( 65535 );

		return root;
	}

	@Override
	public void onCreateOptionsMenu( Menu menu, MenuInflater inflater )
	{
		super.onCreateOptionsMenu( menu, inflater );

		// Inflate the menu; this adds items to the action bar if it is present.
		inflater.inflate( R.menu.menu_enter_connection, menu );
		menu.findItem( R.id.action_qrscanner ).setOnMenuItemClickListener( new OnMenuItemClickListener( )
		{
			@Override
			public boolean onMenuItemClick( MenuItem item )
			{
				Snackbar.make( getActivity( ).findViewById( R.id.coordinatorLayout ),
				               "*todo: open qr code scanner*", Snackbar.LENGTH_SHORT ).show( );
				return false;
			}
		} );
	}
}
