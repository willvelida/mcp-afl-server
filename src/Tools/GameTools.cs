using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class GameTools : BaseAFLTool
    {
        public GameTools(HttpClient httpClient, ILogger<GameTools> logger) : base(httpClient, logger)
        { }

        [McpServerTool, Description("Gets result from a played game")]
        public async Task<List<GameResponse>> GetGameResult(
            [Description("The ID of the game")] int gameId)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("gameId", gameId, val => IsValidId((int)val), "Game ID must be a positive integer")))
            {
                return new List<GameResponse>();
            }

            var endpoint = $"?q=games;game={gameId}";
            var operationName = $"Game {gameId}";

            var result = await ExecuteApiCallAsync<List<GameResponse>>(
                endpoint,
                operationName,
                "games"
            );

            return result ?? new List<GameResponse>();
        }

        [McpServerTool, Description("Get the results from a round of a particular year")]
        public async Task<List<GameResponse>> GetRoundResultsByYear(
            [Description("The year of the round")] int year,
            [Description("The round number")] int round)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                ("round", round, val => IsValidRound((int)val), "Round must be between 1 and 30")))
            {
                return new List<GameResponse>();
            }

            var endpoint = $"?q=games;year={year};round={round}";
            var operationName = $"Games for Year: {year}, Round: {round}";

            var result = await ExecuteApiCallAsync<List<GameResponse>>(
                endpoint,
                operationName,
                "games"
            );

            return result ?? new List<GameResponse>();
        }
    }
}
