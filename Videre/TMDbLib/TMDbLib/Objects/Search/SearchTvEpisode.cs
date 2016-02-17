using System;
using Newtonsoft.Json;

namespace TMDbLib.Objects.Search
{
    public class SearchTvEpisode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("show_id")]
        public int ShowId { get; set; }

        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("air_date")]
        public DateTime? AirDate { get; set; }

        [JsonProperty("still_path")]
        public string StillPath { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }
    }
}