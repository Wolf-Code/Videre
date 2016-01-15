﻿using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace VidereLib
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
        public Grid ControlsGrid { set; get; }

        /// <summary>
        /// The grid containing the media elements, such as the actual <see cref="MediaElement"/> and the subtitles.
        /// </summary>
        public Grid MediaArea { set; get; }

        /// <summary>
        /// The actual <see cref="MediaElement"/> used for playback.
        /// </summary>
        public MediaElement MediaPlayer { set; get; }
    }
}
