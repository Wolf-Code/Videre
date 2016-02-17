﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace TMDbLib.Objects.TvShows
{
    public class TvSeason
    {
        /// <summary>
        /// Object Id, will only be populated when explicitly getting episode details
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("air_date")]
        public DateTime? AirDate { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("episode_count")]
        public int EpisodeCount { get; set; }


        [JsonProperty("episodes")]
        public List<TvEpisode> Episodes { get; set; }

        [JsonProperty("images")]
        public Images Images { get; set; }

        [JsonProperty("credits")]
        public Credits Credits { get; set; }

        [JsonProperty("external_ids")]
        public ExternalIds ExternalIds { get; set; }

        [JsonProperty("videos")]
        public ResultContainer<Video> Videos { get; set; }

        [JsonProperty("account_states")]
        public ResultContainer<TvEpisodeAccountState> AccountStates { get; set; }
    }
}
