
using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Data
{
    /// <summary>
    /// Movie data.
    /// </summary>
    public class MovieData
    {
        /// <summary>
        /// The hash of the movie.
        /// </summary>
        public string MovieHash { private set; get; }

        /// <summary>
        /// The ID on IMDB for the movie.
        /// </summary>
        public string MovieImbdID { private set; get; }

        /// <summary>
        /// The name of the movie.
        /// </summary>
        public string MovieName { private set; get; }

        /// <summary>
        /// The year the movie came out.
        /// </summary>
        public ushort MovieYear { private set; get; }

        /// <summary>
        /// The type of movie, such as movie, tv series or episode.
        /// </summary>
        public string Type { private set; get; }

        /// <summary>
        /// The season of the series.
        /// </summary>
        public ushort SeriesSeason { private set; get; }

        /// <summary>
        /// The episode of the series.
        /// </summary>
        public ushort SeriesEpisode { private set; get; }

        /// <summary>
        /// The seen count.
        /// </summary>
        public ulong SeenCount { private set; get; }

        /// <summary>
        /// The subtitles count.
        /// </summary>
        public uint SubCount { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The struct containing the data.</param>
        public MovieData( XmlRpcStruct output )
        {
            this.MovieHash = ( string ) output[ "MovieHash" ];
            this.MovieImbdID = ( string ) output[ "MovieImdbID" ];
            this.MovieName = ( string ) output[ "MovieName" ];
            this.MovieYear = ushort.Parse( ( string ) output[ "MovieYear" ] );
            this.Type = ( string ) output[ "MovieKind" ];
            this.SeriesSeason = ushort.Parse( ( string ) output[ "SeriesSeason" ] );
            this.SeriesEpisode = ushort.Parse( ( string ) output[ "SeriesEpisode" ] );
            this.SeenCount = ulong.Parse( ( string ) output[ "SeenCount" ] );
            this.SubCount = uint.Parse( ( string ) output[ "SubCount" ] );
        }
    }
}
