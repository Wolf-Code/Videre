﻿using Newtonsoft.Json;

namespace TMDbLib.Objects.General
{
    public class ImageData
    {
        [JsonProperty("file_path")]
        public string FilePath { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso_639_1 { get; set; }

        [JsonProperty("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }
    }
}