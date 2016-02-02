using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using VidereLib.Players;

namespace VidereLib.Data
{
    /// <summary>
    /// Contains the window data to be used by the <see cref="ViderePlayer"/>.
    /// </summary>
    public class WindowData
    {
        /// <summary>
        /// The window in which the player is placed.
        /// </summary>
        public MetroWindow Window { set; get; }

        /// <summary>
        /// The grid containing all controls.
        /// </summary>
        public FrameworkElement MediaControlsContainer { set; get; }

        /// <summary>
        /// The element containing the media elements, such as the actual <see cref="MediaElement"/> and the subtitles.
        /// </summary>
        public FrameworkElement MediaArea { set; get; }

        /// <summary>
        /// The actual element used for playback.
        /// </summary>
        public MediaPlayerBase MediaPlayer { set; get; }
    }
}
