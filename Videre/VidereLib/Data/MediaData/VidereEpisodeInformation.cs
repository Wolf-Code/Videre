using Newtonsoft.Json;
using VidereSubs.OpenSubtitles.Data;

namespace VidereLib.Data.MediaData
{
    /// <summary>
    /// Information about an episode.
    /// </summary>
    public class VidereEpisodeInformation : VidereMediaInformation
    {
        /// <summary>
        /// The season of the show.
        /// </summary>
        [JsonProperty( "season" )]
        public ushort Season { set; get; }

        /// <summary>
        /// The episode of the show.
        /// </summary>
        [JsonProperty( "episode" )]
        public ushort Episode { set; get; }

        /// <summary>
        /// The name of the episode.
        /// </summary>
        [JsonProperty( "episode_name" )]
        public string EpisodeName { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">OpenSubtitles.org information about the episode.</param>
        public VidereEpisodeInformation( MovieData data ) : base( data )
        {
            if ( data == null ) return;

            this.Season = data.SeriesSeason;
            this.Episode = data.SeriesEpisode;
        }
    }
}
