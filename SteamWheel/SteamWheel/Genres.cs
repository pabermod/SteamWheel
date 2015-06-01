using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamWheel
{
    public class Genres
    {
        public string action { get; set; }
        public string adventure { get; set; }
        public string fighting { get; set; }
        [JsonProperty("first-person")]
        public string first_person { get; set; }
        [JsonProperty("Sci-Fi")]
        public string Sci_Fi { get; set; }
        [JsonProperty("Action RPG")]
        public string Action_RPG { get; set; }
        public string flight { get; set; }
        public string party { get; set; }
        public string platformer { get; set; }
        public string puzzle { get; set; }
        public string racing { get; set; }
        [JsonProperty("real-time")]
        public string real_time { get; set; }
        [JsonProperty("role-playing")]
        public string role_playing { get; set; }
        public string simulation { get; set; }
        public string sports { get; set; }
        public string strategy { get; set; }
        [JsonProperty("third-person")]
        public string third_person { get; set; }
        [JsonProperty("turn-based")]
        public string turn_based { get; set; }
        public string wargame { get; set; }
        public string wrestling { get; set; }
    }
}
