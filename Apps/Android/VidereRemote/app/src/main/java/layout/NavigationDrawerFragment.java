package layout;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.widget.Toolbar;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import kevindeelen.wolfcode.videreremote.R;

/**
 * A simple {@link Fragment} subclass.
 */
public class NavigationDrawerFragment extends Fragment
{

	private ActionBarDrawerToggle drawerToggle;
	private DrawerLayout drawerLayout;

	public NavigationDrawerFragment( )
	{
		// Required empty public constructor
	}


	@Override
	public View onCreateView( LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState )
	{
		// Inflate the layout for this fragment
		return inflater.inflate( R.layout.fragment_navigation_drawer, container, false );
	}

	public void initialize( DrawerLayout layout, Toolbar toolbar )
	{
		drawerLayout = layout;
		drawerToggle = new ActionBarDrawerToggle( getActivity( ), layout, toolbar,
		                                          R.string.drawerOpen, R.string.drawerClose )
		{
			@Override
			public void onDrawerOpened( View drawerView )
			{
				super.onDrawerOpened( drawerView );
			}

			@Override
			public void onDrawerClosed( View drawerView )
			{
				super.onDrawerClosed( drawerView );
			}
		};

		drawerLayout.setDrawerListener( drawerToggle );
		drawerLayout.post( new Runnable( )
		{
			@Override
			public void run( )
			{
				drawerToggle.syncState( );
			}
		} );
	}
}
