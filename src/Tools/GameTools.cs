using System.ComponentModel;
using mcp_afl_server.Models;
using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class GameTools : BaseAFLTool
    {
        public GameTools(
            HttpClient httpClient,
            ILogger<GameTools> logger,
            IAuthenticationService authenticationService)
            : base(httpClient, logger, authenticationService)
        { }

        [McpServerTool, Description("Gets result from a played game")]
        public async Task<List<GameResponse>> GetGameResult(
            [Description("The ID of the game")] int gameId)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested game result for game {gameId}");

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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetGameResult: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetGameResult for game {gameId}: {ex.Message}");
                throw;
            }
        }

        [McpServerTool, Description("Get the results from a round of a particular year")]
        public async Task<List<GameResponse>> GetRoundResultsByYear(
            [Description("The year of the round")] int year,
            [Description("The round number")] int round)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested round results for year {year}, round {round}");

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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetRoundResultsByYear: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRoundResultsByYear for year {year}, round {round}: {ex.Message}");
                throw;
            }
        }
    }
}