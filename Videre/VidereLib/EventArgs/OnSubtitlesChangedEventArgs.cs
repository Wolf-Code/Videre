
using VidereSubs;

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
        public SubtitleSegment Subtitles { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="SubtitleSegment">The new subtitles.</param>
        public OnSubtitlesChangedEventArgs( SubtitleSegment SubtitleSegment )
        {
            Subtitles = SubtitleSegment;
        }
    }
}
