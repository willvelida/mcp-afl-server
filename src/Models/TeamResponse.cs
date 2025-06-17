using System.Text.Json.Serialization;

namespace mcp_afl_server.Models
{
    public class TeamResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("abbrev")]
        public string Abbrevation { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("debut")]
        public int Debut { get; set; }
        [JsonPropertyName("retirement")]
        public int Retirement { get; set; }
    }
}
