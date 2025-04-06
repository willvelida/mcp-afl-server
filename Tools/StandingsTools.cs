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
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>("?q=standings");
            var standings = jsonElement.GetProperty("standings").EnumerateArray();

            if (!standings.Any())
            {
                return "Unable to retrieve current standings";
            }

            // Convert JsonElement to StandingsResponse objects and format them
            var standingResponses = standings.Select(s => JsonSerializer.Deserialize<StandingsResponse>(s.ToString()));
            return string.Join("\n--\n", standingResponses.Select(FormatStandingsResponse));
        }

        [McpServerTool, Description("Get the standings for a particular round and year")]
        public static async Task<string> GetStandingsByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the standings")] int year)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=standings;year={year};round={roundNumber}");
            var standings = jsonElement.GetProperty("standings").EnumerateArray();

            if (!standings.Any())
            {
                return $"No standings found for the year {year} after round {roundNumber}";
            }

            // Convert JsonElement to StandingsResponse objects and format them
            var standingResponses = standings.Select(s => JsonSerializer.Deserialize<StandingsResponse>(s.ToString()));
            return string.Join("\n--\n", standingResponses.Select(FormatStandingsResponse));
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
