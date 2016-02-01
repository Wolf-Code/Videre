package adapters;

import android.support.v7.widget.RecyclerView.Adapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.List;

import containers.NavigationDrawerEntryContainer;
import kevindeelen.wolfcode.videreremote.R;
import viewholders.NavigationDrawerEntryViewHolder;

/**
 * Created by Wolf on 01-Feb-16.
 */
public class NavigationDrawerAdapter extends Adapter<NavigationDrawerEntryViewHolder>
{
	private List<NavigationDrawerEntryContainer> entries;

	public NavigationDrawerAdapter( List<NavigationDrawerEntryContainer> entries )
	{
		this.entries = entries;
	}

	@Override
	public NavigationDrawerEntryViewHolder onCreateViewHolder( ViewGroup parent, int viewType )
	{
		View view = LayoutInflater.from( parent.getContext( ) ).inflate( R.layout.view_navigation_drawer_entry, parent, false );
		return new NavigationDrawerEntryViewHolder( view );
	}

	@Override
	public void onBindViewHolder( NavigationDrawerEntryViewHolder holder, int position )
	{
		NavigationDrawerEntryContainer container = entries.get( position );

		holder.setContainer( container );
		holder.title.setText( container.title );
	}

	@Override
	public int getItemCount( )
	{
		return entries.size( );
	}
}
