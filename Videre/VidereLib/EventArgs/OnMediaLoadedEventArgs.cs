using System.IO;

namespace VidereLib.EventArgs
{
    public class OnMediaLoadedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The media file that has been loaded.
        /// </summary>
        public FileInfo MediaFile { private set; get; }

        public OnMediaLoadedEventArgs( FileInfo file )
        {
            this.MediaFile = file;
        }
    }
}
