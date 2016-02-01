package layout;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.RecyclerView.Adapter;
import android.support.v7.widget.Toolbar;

import java.util.ArrayList;

import adapters.NavigationDrawerAdapter;
import containers.NavigationDrawerEntryContainer;
import kevindeelen.wolfcode.videreremote.R;

public class MainActivity extends AppCompatActivity
{
	ArrayList<NavigationDrawerEntryContainer> navigationContainers;

	@Override
	protected void onCreate( Bundle savedInstanceState )
	{
		super.onCreate( savedInstanceState );
		setContentView( R.layout.activity_main_appbar );

		Toolbar toolbar = ( Toolbar ) findViewById( R.id.toolbar );
		setSupportActionBar( toolbar );

		NavigationDrawerFragment drawerFragment = ( NavigationDrawerFragment ) getSupportFragmentManager( ).findFragmentById( R.id.fragment_navdrawer );
		drawerFragment.initialize( ( DrawerLayout ) findViewById( R.id.drawerLayout ), toolbar );

		RecyclerView navDrawerRecycler = ( RecyclerView ) findViewById( R.id.navDrawerRecyclerView );
		navigationContainers = new ArrayList<>( );
		NavigationDrawerEntryContainer connectorEntry = new NavigationDrawerEntryContainer( )
		{
			@Override
			public Fragment getFragment( )
			{
				return new EnterConnectionFragment( );
			}
		};
		connectorEntry.title = "Connector";

		navigationContainers.add( connectorEntry );

		Adapter adapter = new NavigationDrawerAdapter( navigationContainers );
		navDrawerRecycler.setAdapter( adapter );
		navDrawerRecycler.setLayoutManager( new LinearLayoutManager( this ) );

		getSupportActionBar( ).setDisplayShowHomeEnabled( true );
	}
}
