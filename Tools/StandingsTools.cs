using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class StandingsTools
    {
        [McpServerTool, Description("Gets the current standing")]
        public static async Task<string> GetCurrentStandings(HttpClient httpClient)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>("?q=standings");
            var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(
                response.GetProperty("standings").GetRawText());

            if (!standingsResponse.Any() || standingsResponse == null)
            {
                return "Unable to retrieve current standings";
            }
           
            return string.Join("\n--\n", standingsResponse.Select(standings => FormatStandingsResponse(standings)));
        }

        [McpServerTool, Description("Get the standings for a particular round and year")]
        public static async Task<string> GetStandingsByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the standings")] int year)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=standings;year={year};round={roundNumber}");
            var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(
                response.GetProperty("standings").GetRawText());

            if (!standingsResponse.Any() || standingsResponse == null)
            {
                return $"No standings found for the year {year} after round {roundNumber}";
            }

            return string.Join("\n--\n", standingsResponse.Select(standings => FormatStandingsResponse(standings)));
        }

        private static string FormatStandingsResponse(StandingsResponse standing)
        {
            return $"""
                Rank: {standing.Rank}
                Name: {standing.TeamName}
                Played: {standing.Played}
                Wins: {standing.Wins}
                Draws: {standing.Draws}
                Losses: {standing.Losses}
                For: {standing.For}
                Against: {standing.Against}
                Points: {standing.Points}
                Goals For: {standing.GoalsFor}
                Goals Against: {standing.GoalsAgainst}
                Behinds For: {standing.BehindsFor}
                Behinds Against: {standing.BehindsAgainst}
                Percentage: {standing.Percentage}
            """;
        }
    }
}
