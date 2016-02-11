
namespace VidereLib.Data
{
    /// <summary>
    /// Some information about a movie.
    /// </summary>
    public class MovieInformation
    {
        /// <summary>
        /// The name of the movie.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// The year the movie came out.
        /// </summary>
        public ushort Year { set; get; }

        /// <summary>
        /// The IMDB ID of the movie.
        /// </summary>
        public string IMDBID { set; get; }

        /// <summary>
        /// The poster URL of the movie.
        /// </summary>
        public string Poster { set; get; }

        /// <summary>
        /// The opensubtitles.org hash of the movie.
        /// </summary>
        public string Hash { set; get; }

        /// <summary>
        /// The rating of the movie on themoviedb.org.
        /// </summary>
        public decimal Rating { set; get; }
    }
}
