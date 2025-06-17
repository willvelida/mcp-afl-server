using System.Text.Json.Serialization;

namespace mcp_afl_server.Models
{
    public class PowerRankingsResponse
    {
        [JsonPropertyName("sourceid")]
        public int SourceId { get; set; }
        [JsonPropertyName("power")]
        public string Power { get; set; }
        [JsonPropertyName("team")]
        public string Team { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
        [JsonPropertyName("teamid")]
        public int TeamId { get; set; }
        [JsonPropertyName("dummy")]
        public int Dummy { get; set; }
        [JsonPropertyName("updated")]
        public string Updated { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("round")]
        public int Round { get; set; }
    }
}
