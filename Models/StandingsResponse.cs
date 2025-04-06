using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace mcp_afl_server.Models
{
    public class StandingsResponse
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
        [JsonPropertyName("behinds_for")]
        public int BehindsFor { get; set; }
        [JsonPropertyName("goals_for")]
        public int GoalsFor { get; set; }
        [JsonPropertyName("behinds_against")]
        public int BehindsAgainst { get; set; }
        [JsonPropertyName("goals_against")]
        public int GoalsAgainst { get; set; }
        [JsonPropertyName("pts")]
        public int Points { get; set; }
        [JsonPropertyName("wins")]
        public int Wins { get; set; }
        [JsonPropertyName("draws")]
        public int Draws { get; set; }
        [JsonPropertyName("losses")]
        public int Losses { get; set; }
        [JsonPropertyName("played")]
        public int Played { get; set; }
        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
        [JsonPropertyName("for")]
        public int For { get; set; }
        [JsonPropertyName("against")]
        public int Against { get; set; }
        [JsonPropertyName("name")]
        public string TeamName { get; set; }

    }
}
