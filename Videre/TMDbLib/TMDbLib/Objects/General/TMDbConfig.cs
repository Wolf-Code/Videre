﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TMDbLib.Objects.General
{
    [Serializable]
    public class TMDbConfig
    {
        [JsonProperty("images")]
        public ConfigImageTypes Images { get; set; }

        [JsonProperty("change_keys")]
        public List<string> ChangeKeys { get; set; }
    }
}