using Newtonsoft.Json;
using VidereSubs.OpenSubtitles.Data;

namespace VidereLib.Data
{
    /// <summary>
    /// Information about the actual media (movie, episode, etc.)
    /// </summary>
    public class VidereMediaInformation
    {
        /// <summary>
        /// The name of the movie.
        /// </summary>
        [JsonProperty( "name" )]
        public string Name { set; get; }

        /// <summary>
        /// The IMDB ID of the movie.
        /// </summary>
        [JsonProperty( "IMDBID" )]
        public string IMDBID { set; get; }

        /// <summary>
        /// The year the movie came out.
        /// </summary>
        [JsonProperty( "year" )]
        public ushort Year { set; get; }

        /// <summary>
        /// The poster URL of the movie.
        /// </summary>
        [JsonProperty( "poster" )]
        public string Poster { set; get; }

        /// <summary>
        /// The opensubtitles.org hash of the movie.
        /// </summary>
        [JsonProperty( "hash" )]
        public string Hash { set; get; }

        /// <summary>
        /// The rating of the movie on themoviedb.org.
        /// </summary>
        [JsonProperty( "rating" )]
        public decimal Rating { set; get; }

        /// <summary>
        /// The type of movie.
        /// </summary>
        public MovieData.MovieKind MovieType { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The movie data.</param>
        public VidereMediaInformation( MovieData data )
        {
            if ( data == null ) return;

            Hash = data.MovieHash;
            IMDBID = data.MovieImbdID;
            Name = data.MovieName;
            Year = data.MovieYear;
            MovieType = data.MovieType;
        }
    }
}