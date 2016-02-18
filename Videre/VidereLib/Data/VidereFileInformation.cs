using System;

namespace VidereLib.Data
{
    /// <summary>
    /// Information about file properties of the media.
    /// </summary>
    public class VidereFileInformation
    {
        /// <summary>
        /// The duration of the media.
        /// </summary>
        public TimeSpan Duration { set; get; }

        /// <summary>
        /// The media's height.
        /// </summary>
        public uint Width { set; get; }

        /// <summary>
        /// The media's width.
        /// </summary>
        public uint Height { set; get; }
    }
}
