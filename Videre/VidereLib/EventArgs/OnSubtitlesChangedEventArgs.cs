
namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when the current subtitles should be changed.
    /// </summary>
    public class OnSubtitlesChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The new subtitles.
        /// </summary>
        public SubtitleData Subtitles { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="subtitleData">The new subtitles.</param>
        public OnSubtitlesChangedEventArgs( SubtitleData subtitleData )
        {
            Subtitles = subtitleData;
        }
    }
}
