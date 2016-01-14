using System.IO;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when media has been loaded.
    /// </summary>
    public class OnMediaLoadedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The media file that has been loaded.
        /// </summary>
        public FileInfo MediaFile { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> of the loaded media.</param>
        public OnMediaLoadedEventArgs( FileInfo file )
        {
            this.MediaFile = file;
        }
    }
}
