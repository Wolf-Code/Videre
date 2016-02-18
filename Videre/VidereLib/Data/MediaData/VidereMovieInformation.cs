using VidereSubs.OpenSubtitles.Data;

namespace VidereLib.Data.MediaData
{
    /// <summary>
    /// Information about a movie.
    /// </summary>
    public class VidereMovieInformation : VidereMediaInformation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">OpenSubtitles.org information about a movie.</param>
        public VidereMovieInformation( MovieData data ) : base( data )
        {
            if ( data == null ) return;
        }
    }
}
