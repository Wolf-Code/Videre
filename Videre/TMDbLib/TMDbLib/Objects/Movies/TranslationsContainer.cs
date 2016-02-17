﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace TMDbLib.Objects.Movies
{
    // TODO: Move into generic objects
    public class TranslationsContainer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}