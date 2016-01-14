
namespace VidereLib.EventArgs
{
    public class OnFullscreenChangedEventArgs : System.EventArgs
    {
        public bool IsFullScreen { private set; get; }

        public OnFullscreenChangedEventArgs( bool isFullScreen )
        {
            this.IsFullScreen = isFullScreen;
        }
    }
}
