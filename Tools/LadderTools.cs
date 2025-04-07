using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class LadderTools
    {
        [McpServerTool, Description("Get the projected ladder for a particular round and year")]
        public static async Task<string> GetProjectedLadderByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=ladder;year={year};round={roundNumber}");
            var sourcesResponse = JsonSerializer.Deserialize<List<LadderResponse>>(
                response.GetProperty("ladder").GetRawText());

            if (sourcesResponse == null || !sourcesResponse.Any())
            {
                return $"No ladder found for the year {year} after round {roundNumber}";
            }
            return string.Join("\n--\n", sourcesResponse.Select(ladder => FormatLadderResponse(ladder)));
        }

        [McpServerTool, Description("Get the projected ladder for a particular round and year by source")]
        public static async Task<string> GetProjectedLadderByRoundAndYearBySource(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year,
            [Description("The source of the ladder")] string source)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=ladder;year={year};round={roundNumber};source={source}");
            var sourcesResponse = JsonSerializer.Deserialize<List<LadderResponse>>(
                response.GetProperty("ladder").GetRawText());

            if (sourcesResponse == null || !sourcesResponse.Any())
            {
                return $"No ladder found for the year {year} after round {roundNumber} from the source {source}";
            }

            return string.Join("\n--\n", sourcesResponse.Select(ladder => FormatLadderResponse(ladder)));
        }

        private static string FormatLadderResponse(LadderResponse ladderResponse)
        {
            return $"""
                Team: {ladderResponse.Team}
                Swarms: {string.Join(", ", ladderResponse.Swarms)}
                Wins: {ladderResponse.Wins}
                Team ID: {ladderResponse.TeamId}
                Updated: {ladderResponse.Updated}
                Dummy: {ladderResponse.Dummy}
                Year: {ladderResponse.Year}
                Round: {ladderResponse.Round}
                Source ID: {ladderResponse.SourceId}
                Mean Rank: {ladderResponse.MeanRank}
                Source: {ladderResponse.Source}
                Rank: {ladderResponse.Rank}
                """;
        }
    }
}
