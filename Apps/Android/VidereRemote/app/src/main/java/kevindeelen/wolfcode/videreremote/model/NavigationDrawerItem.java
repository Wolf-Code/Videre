package kevindeelen.wolfcode.videreremote.model;

/**
 * Created by Wolf on 23-Jan-16.
 */
public class NavigationDrawerItem
{
    private boolean showNotify;
    private String title;


    public NavigationDrawerItem ( )
    {

    }

    public NavigationDrawerItem ( boolean showNotify, String title )
    {
        this.showNotify = showNotify;
        this.title = title;
    }

    public boolean isShowNotify ( )
    {
        return showNotify;
    }

    public void setShowNotify ( boolean showNotify )
    {
        this.showNotify = showNotify;
    }

    /**
     * @return The item's title.
     */
    public String getTitle ( )
    {
        return title;
    }

    /**
     * @param title Sets the item's title.
     */
    public void setTitle ( String title )
    {
        this.title = title;
    }
}
