﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace TMDbLib.Objects.Changes
{
    public class Change
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("items")]
        public List<ChangeItem> Items { get; set; }
    }
}