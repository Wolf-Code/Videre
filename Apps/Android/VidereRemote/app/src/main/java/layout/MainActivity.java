package layout;

import android.os.Bundle;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;

import kevindeelen.wolfcode.videreremote.R;

public class MainActivity extends AppCompatActivity
{

	@Override
	protected void onCreate( Bundle savedInstanceState )
	{
		super.onCreate( savedInstanceState );
		setContentView( R.layout.activity_main );

		Toolbar toolbar = ( Toolbar ) findViewById( R.id.toolbar );
		setSupportActionBar( toolbar );

		NavigationDrawerFragment drawerFragment = ( NavigationDrawerFragment ) getSupportFragmentManager( ).findFragmentById( R.id.fragment_navdrawer );
		drawerFragment.initialize( ( DrawerLayout ) findViewById( R.id.drawerLayout ), toolbar );

		getSupportActionBar().setDisplayShowHomeEnabled( true );
	}
}
