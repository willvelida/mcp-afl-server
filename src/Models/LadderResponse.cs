using System.Text.Json.Serialization;

namespace mcp_afl_server.Models
{
    public class LadderResponse
    {
        [JsonPropertyName("team")]
        public string Team { get; set; }
        [JsonPropertyName("swarms")]
        public List<string> Swarms { get; set; }
        [JsonPropertyName("wins")]
        public string Wins { get; set; }
        [JsonPropertyName("teamid")]
        public int TeamId { get; set; }
        [JsonPropertyName("updated")]
        public string? Updated { get; set; }
        [JsonPropertyName("dummy")]
        public int Dummy { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("round")]
        public int Round { get; set; }
        [JsonPropertyName("sourceid")]
        public int SourceId { get; set; }
        [JsonPropertyName("mean_rank")]
        public string MeanRank { get; set; }
        [JsonPropertyName("source")]
        public string? Source { get; set; }
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}
