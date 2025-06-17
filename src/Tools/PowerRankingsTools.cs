using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class PowerRankingsTools
    {
        [McpServerTool, Description("Get Power Ranking by Round and Year")]
        public static async Task<string> GetPowerRankingByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=power;year={year};round={roundNumber}");
            var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                response.GetProperty("power").GetRawText());

            if (!powerRankingResponse.Any() || powerRankingResponse == null)
            {
                return $"Unable to retrieve power ranking for Round {roundNumber} in year {year}";
            }

            return string.Join("\n--\n", powerRankingResponse.Select(pow => FormatPowerRankingsResponse(pow)));
        }

        [McpServerTool, Description("Get Power Ranking by Round, Year, and Model Source")]
        public static async Task<string> GetPowerRankingByRoundYearAndSource(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year,
            [Description("The source ID of the model")] int sourceId)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=power;year={year};round={roundNumber};source={sourceId}");
            var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                response.GetProperty("power").GetRawText());

            if (!powerRankingResponse.Any() || powerRankingResponse == null)
            {
                return $"Unable to retrieve power ranking for Round {roundNumber} in year {year} for Source Id {sourceId}";
            }

            return string.Join("\n--\n", powerRankingResponse.Select(pow => FormatPowerRankingsResponse(pow)));
        }

        [McpServerTool, Description("Get Power Ranking for Team by Round, Year, and Model Source")]
        public static async Task<string> GetTeamPowerRankingByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year,
            [Description("The Team Id")] int teamId)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=power;year={year};round={roundNumber};team={teamId}");
            var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                response.GetProperty("power").GetRawText());

            if (!powerRankingResponse.Any() || powerRankingResponse == null)
            {
                return $"Unable to retrieve power ranking for Round {roundNumber} in year {year} for Team Id {teamId}";
            }

            return string.Join("\n--\n", powerRankingResponse.Select(pow => FormatPowerRankingsResponse(pow)));
        }

        private static string FormatPowerRankingsResponse(PowerRankingsResponse powerRankingsResponse)
        {
            return $"""
                Team: {powerRankingsResponse.Team}
                Power: {powerRankingsResponse.Power}
                Rank: {powerRankingsResponse.Rank}
                Team ID: {powerRankingsResponse.TeamId}
                Source: {powerRankingsResponse.Source}
                Source ID: {powerRankingsResponse.SourceId}
                Year: {powerRankingsResponse.Year}
                Round: {powerRankingsResponse.Round}
                Updated: {powerRankingsResponse.Updated}
                Dummy: {powerRankingsResponse.Dummy}
                """;
        }
    }
}
