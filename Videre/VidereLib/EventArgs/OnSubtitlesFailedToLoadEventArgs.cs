using System.IO;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when subtitles failed to load.
    /// </summary>
    public class OnSubtitlesFailedToLoadEventArgs : System.EventArgs
    {
        /// <summary>
        /// The subtitles file which had failed to load.
        /// </summary>
        public FileInfo Subtitles { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">The subtitles file.</param>
        public OnSubtitlesFailedToLoadEventArgs( FileInfo info )
        {
            Subtitles = info;
        }
    }
}
