using VidereSubs;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when subtitles have been unloaded.
    /// </summary>
    public class OnSubtitlesUnloadedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The subtitles file which has been unloaded.
        /// </summary>
        public Subtitles Subtitles { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="subs">The subtitles file.</param>
        public OnSubtitlesUnloadedEventArgs( Subtitles subs )
        {
            Subtitles = subs;
        }
    }
}
