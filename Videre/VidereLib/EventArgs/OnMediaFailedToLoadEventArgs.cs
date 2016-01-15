using System;
using System.IO;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when media failed to load.
    /// </summary>
    public class OnMediaFailedToLoadEventArgs : System.EventArgs
    {
        /// <summary>
        /// The media file that failed to load.
        /// </summary>
        public FileInfo MediaFile { private set; get; }

        /// <summary>
        /// The exception.
        /// </summary>
        public Exception Exception { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="file">The <see cref="FileInfo"/> of the media that failed to load.</param>
        public OnMediaFailedToLoadEventArgs( Exception exception, FileInfo file )
        {
            this.Exception = exception;
            this.MediaFile = file;
        }
    }
}
