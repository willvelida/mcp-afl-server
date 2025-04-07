using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class TipsTools
    {
        [McpServerTool, Description("Get the tips for a particular round and year")]
        public static async Task<string> GetTipsByRoundAndYear(
            HttpClient httpClient,
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the tips")] int year)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=tips;year={year};round={roundNumber}");
            var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                response.GetProperty("tips").GetRawText());

            if (!tipsResponse.Any() || tipsResponse == null)
            {
                return $"No tips found for the year {year} after round {roundNumber}";
            }

            return string.Join("\n--\n", tipsResponse.Select(tips => FormatTipsResponse(tips)));
        }

        [McpServerTool, Description("Get the tips of a particular game")]
        public static async Task<string> GetTipsByGame(
            HttpClient httpClient,
            [Description("The ID of the game")] int gameId)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=tips;game={gameId}");
            var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                response.GetProperty("tips").GetRawText());
            if (!tipsResponse.Any() || tipsResponse == null)
            {
                return "No tips found for this game";
            }
            return string.Join("\n--\n", tipsResponse.Select(tips => FormatTipsResponse(tips)));
        }

        [McpServerTool, Description("Get the tips for current and future games")]
        public static async Task<string> GetFutureTips(HttpClient httpClient)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>("?q=tips;complete=!100");
            var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                response.GetProperty("tips").GetRawText());

            if (!tipsResponse.Any() || tipsResponse == null)
            {
                return "No future tips found";
            }

            return string.Join("\n--\n", tipsResponse.Select(tips => FormatTipsResponse(tips)));
        }

        private static string FormatTipsResponse(TipsResponse tipsResponse)
        {
            return $"Correct: {tipsResponse.Correct}, Round: {tipsResponse.Round}, Year: {tipsResponse.Year}, " +
                   $"Updated: {tipsResponse.Updated}, Bits: {tipsResponse.Bits}, Error: {tipsResponse.Error}, " +
                   $"GameId: {tipsResponse.GameId}, Source: {tipsResponse.Source}, TipTeamId: {tipsResponse.TipTeamId}, " +
                   $"AwayTeam: {tipsResponse.AwayTeam}, HomeTeam: {tipsResponse.HomeTeam}, HomeMargin: {tipsResponse.HomeMargin}, " +
                   $"SourceId: {tipsResponse.SourceId}, Margin: {tipsResponse.Margin}, Venue: {tipsResponse.Venue}, " +
                   $"HomeTeamId: {tipsResponse.HomeTeamId}, AwayTeamId: {tipsResponse.AwayTeamId}, Date: {tipsResponse.Date}, " +
                   $"Tip: {tipsResponse.Tip}, HomeConfidence: {tipsResponse.HomeConfidence}, Confidence: {tipsResponse.Confidence}";
        }
    }
}
