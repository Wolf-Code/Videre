
namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when the fullscreen state is changed.
    /// </summary>
    public class OnFullscreenChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Indicates whether the new state is in fullscreen or not.
        /// </summary>
        public bool IsFullScreen { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isFullScreen">True if the new state is fullscreen, false otherwise.</param>
        public OnFullscreenChangedEventArgs( bool isFullScreen )
        {
            IsFullScreen = isFullScreen;
        }
    }
}
