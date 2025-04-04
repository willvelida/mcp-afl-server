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
    public static class StandingsTools
    {
        [McpServerTool, Description("Gets the current standing")]
        public static async Task<string> GetCurrentStandings(
            HttpClient httpClient)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>("?q=standings");
            var standings = jsonElement.GetProperty("standings").EnumerateArray();

            if (!standings.Any())
            {
                return "Unable to retrieve current standings";
            }

            return string.Join("\n--\n", standings.Select(standing =>
            {
                return $"""
                    Rank: {standing.GetProperty("rank").GetInt32()}
                    Name: {standing.GetProperty("name").GetString()}
                    Played: {standing.GetProperty("played").GetInt32()}
                    Wins: {standing.GetProperty("wins").GetInt32()}
                    Draws: {standing.GetProperty("draws").GetInt32()}
                    Losses: {standing.GetProperty("losses").GetInt32()}
                    For: {standing.GetProperty("for").GetInt32()}
                    Against: {standing.GetProperty("against").GetInt32()}
                    Points: {standing.GetProperty("pts").GetInt32()}
                    Goals For: {standing.GetProperty("goals_for").GetInt32()}
                    Goals Against: {standing.GetProperty("goals_against").GetInt32()}
                    Behinds For: {standing.GetProperty("behinds_for").GetInt32()}
                    Behinds Against: {standing.GetProperty("behinds_against").GetInt32()}
                    Percentage: {standing.GetProperty("percentage").GetDouble()}
                """;
            }));
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

            return string.Join("\n--\n", standings.Select(standing =>
            {
                return $"""
                    Rank: {standing.GetProperty("rank").GetInt32()}
                    Name: {standing.GetProperty("name").GetString()}
                    Played: {standing.GetProperty("played").GetInt32()}
                    Wins: {standing.GetProperty("wins").GetInt32()}
                    Draws: {standing.GetProperty("draws").GetInt32()}
                    Losses: {standing.GetProperty("losses").GetInt32()}
                    For: {standing.GetProperty("for").GetInt32()}
                    Against: {standing.GetProperty("against").GetInt32()}
                    Points: {standing.GetProperty("pts").GetInt32()}
                    Goals For: {standing.GetProperty("goals_for").GetInt32()}
                    Goals Against: {standing.GetProperty("goals_against").GetInt32()}
                    Behinds For: {standing.GetProperty("behinds_for").GetInt32()}
                    Behinds Against: {standing.GetProperty("behinds_against").GetInt32()}
                    Percentage: {standing.GetProperty("percentage").GetDouble()}
                """;
            }));
        }
    }
}
