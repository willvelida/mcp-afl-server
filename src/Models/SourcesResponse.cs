using System.Text.Json.Serialization;

namespace mcp_afl_server.Models
{
    public class SourcesResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("int")]
        public int Id { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }
}
