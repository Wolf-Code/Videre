using System;
using System.IO;

namespace VidereLib.Data
{
    /// <summary>
    /// A container class for information about a media file.
    /// </summary>
    public class VidereMedia
    {
        /// <summary>
        /// The file containing the media.
        /// </summary>
        public FileInfo File { set; get; }

        /// <summary>
        /// The duration of the media.
        /// </summary>
        public TimeSpan Duration { set; get; }

        /// <summary>
        /// The name of the media.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> of the media.</param>
        public VidereMedia( FileInfo file )
        {
            this.File = file;
        }
    }
}
