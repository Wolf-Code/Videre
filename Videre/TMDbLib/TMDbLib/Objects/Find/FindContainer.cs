using Newtonsoft.Json;
using System.Collections.Generic;
using TMDbLib.Objects.General;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace TMDbLib.Objects.Find
{
    public class FindContainer
    {
        [JsonProperty( "movie_results" )]
        public List<MovieResult> MovieResults { get; set; }

        [JsonProperty( "person_results" )]
        public List<Person> PersonResults { get; set; } // Unconfirmed type

        [JsonProperty( "tv_results" )]
        public List<SearchTv> TvResults { get; set; }

        [JsonProperty( "tv_episode_results" )]
        public List<SearchTvEpisode> TvEpisodes { get; set; }

        [JsonProperty( "tv_season_results" )]
        public List<TvSeason> TvSeasons { get; set; }
    }
}