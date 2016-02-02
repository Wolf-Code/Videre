using System.IO;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when subtitles have been loaded.
    /// </summary>
    public class OnSubtitlesLoadedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The subtitles file which has been loaded.
        /// </summary>
        public FileInfo Subtitles { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">The subtitles file.</param>
        public OnSubtitlesLoadedEventArgs( FileInfo info )
        {
            this.Subtitles = info;
        }
    }
}
