package viewholders;

import android.support.v4.app.FragmentManager;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.RecyclerView.ViewHolder;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.TextView;

import containers.NavigationDrawerEntryContainer;
import kevindeelen.wolfcode.videreremote.R;

/**
 * Created by Wolf on 01-Feb-16.
 */
public class NavigationDrawerEntryViewHolder extends ViewHolder implements OnClickListener
{
	public TextView title;
	NavigationDrawerEntryContainer container;

	public NavigationDrawerEntryViewHolder( View itemView )
	{
		super( itemView );

		itemView.setOnClickListener( this );
		title = ( TextView ) itemView.findViewById( R.id.navItemText );
	}

	public void setContainer( NavigationDrawerEntryContainer container )
	{
		this.container = container;
	}

	@Override
	public void onClick( View v )
	{
		FragmentManager fragmentManager = ( ( AppCompatActivity ) v.getContext( ) ).getSupportFragmentManager( );
		fragmentManager.beginTransaction( ).replace( R.id.drawerLayout, container.getFragment( ) ).commit( );
	}
}
