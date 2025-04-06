using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace mcp_afl_server.Models
{
    public class GameResponse
    {
        [JsonPropertyName("hteam")]
        public string HomeTeam { get; set; }
        [JsonPropertyName("ateam")]
        public string AwayTeam { get; set; }
        [JsonPropertyName("hscore")]
        public int HomeTeamScore { get; set; }
        [JsonPropertyName("hgoals")]
        public int HomeTealGoals { get; set; }
        [JsonPropertyName("hbehinds")]
        public int HomeTeamBehinds { get; set; }
        [JsonPropertyName("ascore")]
        public int AwayTeamScore { get; set; }
        [JsonPropertyName("agoals")]
        public int AwayTeamGoals { get; set; }
        [JsonPropertyName("abehinds")]
        public int AwayTeamBehinds { get; set; }
        [JsonPropertyName("venue")]
        public string Venue { get; set; }
        [JsonPropertyName("round")]
        public int Round { get; set; }
        [JsonPropertyName("roundname")]
        public string RoundName { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("winner")]
        public string Winner { get; set; }
        [JsonPropertyName("is_final")]
        public int FinalGameType { get; set; }
        [JsonPropertyName("is_grand_final")]
        public int IsGrandFinal { get; set; }
    }
}
