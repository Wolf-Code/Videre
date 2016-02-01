using VidereLib.Data;

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
        public VidereMedia MediaFile { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media">The <see cref="VidereMedia"/> of the loaded media.</param>
        public OnMediaLoadedEventArgs( VidereMedia media )
        {
            this.MediaFile = media;
        }
    }
}
