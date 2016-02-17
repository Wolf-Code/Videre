﻿using Newtonsoft.Json;

namespace TMDbLib.Objects.Lists
{
    internal class ListCreateReply
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}
