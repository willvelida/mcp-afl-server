using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class GameTools
    {
        [McpServerTool, Description("Gets result from a played game")]
        public static async Task<string> GetGameResult(
            HttpClient httpClient,
            [Description("The ID of the game")] int gameId)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=games;game={gameId}");
            var gameResponses = JsonSerializer.Deserialize<List<GameResponse>>(
                response.GetProperty("games").GetRawText());

            if (gameResponses == null || !gameResponses.Any())
            {
                return "No game found!";
            }

            return string.Join("\n--\n", gameResponses.Select(game => FormatGameResponse(game)));
        }

        [McpServerTool, Description("Get the results from a round of a particular year")]
        public static async Task<string> GetRoundResultsByYear(
            HttpClient httpClient,
            [Description("The year of the round")] int year,
            [Description("The round number")] int round)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=games;year={year};round={round}");
            var gameResponses = JsonSerializer.Deserialize<List<GameResponse>>(
                response.GetProperty("games").GetRawText());

            if (gameResponses == null || !gameResponses.Any())
            {
                return $"No games found for Round {round} in the year {year}";
            }

            return string.Join("\n--\n", gameResponses.Select(game => FormatGameResponse(game)));
        }

        private static string FormatGameResponse(GameResponse game)
        {
            return $"""
                Home Team: {game.HomeTeam}
                Away Team: {game.AwayTeam}
                Home Team Score: {game.HomeTeamScore}
                Home Team Goals: {game.HomeTealGoals}
                Home Team Behinds: {game.HomeTeamBehinds}
                Away Team Score: {game.AwayTeamScore}
                Away Team Goals: {game.AwayTeamGoals}
                Away Team Behinds: {game.AwayTeamBehinds}
                Venue: {game.Venue}
                Round: {game.Round}
                Round Name: {game.RoundName}
                Date: {game.Date}
                Winner: {game.Winner}
            """;
        }
    }
}
