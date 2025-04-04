using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=games;game={gameId}");
            var games = jsonElement.GetProperty("games").EnumerateArray();

            if (!games.Any())
            {
                return "No game found!";
            }

            return string.Join("\n--\n", games.Select(game =>
            {
                return $"""
                    Home Team: {game.GetProperty("hteam").GetString()}
                    Away Team: {game.GetProperty("ateam").GetString()}
                    Home Team Score: {game.GetProperty("hscore").GetInt32()}
                    Home Team Goals: {game.GetProperty("hgoals").GetInt32()}
                    Home Team Behinds: {game.GetProperty("hbehinds").GetInt32()}
                    Away Team Score: {game.GetProperty("ascore").GetInt32()}
                    Away Team Goals: {game.GetProperty("agoals").GetInt32()}
                    Away Team Behinds: {game.GetProperty("abehinds").GetInt32()}
                    Venue: {game.GetProperty("venue").GetString()}
                    Round: {game.GetProperty("round").GetString()}
                    Date: {game.GetProperty("date").GetString()}
                    Winner: {game.GetProperty("winner").GetString()}
                """;
            }));
        }

        [McpServerTool, Description("Get the results from a round of a particular year")]
        public static async Task<string> GetRoundResultsByYear(
            HttpClient httpClient,
            [Description("The year of the round")] int year,
            [Description("The round number")] int round)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=games;year={year};round={round}");
            var games = jsonElement.GetProperty("games").EnumerateArray();

            if (!games.Any())
            {
                return $"No games found for Round {round} in the year {year}";
            }

            return string.Join("\n--\n", games.Select(game =>
            {
                return $"""
                    Home Team: {game.GetProperty("hteam").GetString()}
                    Away Team: {game.GetProperty("ateam").GetString()}
                    Home Team Score: {game.GetProperty("hscore").GetInt32()}
                    Home Team Goals: {game.GetProperty("hgoals").GetInt32()}
                    Home Team Behinds: {game.GetProperty("hbehinds").GetInt32()}
                    Away Team Score: {game.GetProperty("ascore").GetInt32()}
                    Away Team Goals: {game.GetProperty("agoals").GetInt32()}
                    Away Team Behinds: {game.GetProperty("abehinds").GetInt32()}
                    Venue: {game.GetProperty("venue").GetString()}
                    Round: {game.GetProperty("round").GetString()}
                    Date: {game.GetProperty("date").GetString()}
                    Winner: {game.GetProperty("winner").GetString()}
                """;
            }));
        }
    }
}
