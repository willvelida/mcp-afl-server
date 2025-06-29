using System.ComponentModel;
using mcp_afl_server.Models;
using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class TipsTools : BaseAFLTool
    {
        public TipsTools(
            HttpClient httpClient,
            ILogger<TipsTools> logger,
            IAuthenticationService authenticationService)
            : base(httpClient, logger, authenticationService)
        {
        }

        [McpServerTool, Description("Get the tips for a particular round and year")]
        public async Task<List<TipsResponse>> GetTipsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the tips")] int year)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation("User {UserId} requested tips for year {Year}, round {Round}",
                    user?.Id, year, roundNumber);

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                    ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
                {
                    return new List<TipsResponse>();
                }

                var endpoint = $"?q=tips;year={year};round={roundNumber}";
                var operationName = $"Tips for Year: {year}, Round: {roundNumber}";

                var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                    endpoint,
                    operationName,
                    "tips"
                );

                return result ?? new List<TipsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for GetTipsByRoundAndYear");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTipsByRoundAndYear for year {Year}, round {Round}", year, roundNumber);
                throw;
            }
        }

        [McpServerTool, Description("Get the tips of a particular game")]
        public async Task<List<TipsResponse>> GetTipsByGame(
            [Description("The ID of the game")] int gameId)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation("User {UserId} requested tips for game {GameId}",
                    user?.Id, gameId);

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("gameId", gameId, val => IsValidId((int)val), "Game ID must be a positive integer")))
                {
                    return new List<TipsResponse>();
                }

                var endpoint = $"?q=tips;game={gameId}";
                var operationName = $"Tips for Game ID: {gameId}";

                var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                    endpoint,
                    operationName,
                    "tips"
                );

                return result ?? new List<TipsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for GetTipsByGame");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTipsByGame for game {GameId}", gameId);
                throw;
            }
        }

        [McpServerTool, Description("Get the tips for current and future games")]
        public async Task<List<TipsResponse>> GetFutureTips()
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation("User {UserId} requested future tips", user?.Id);

                const string endpoint = "?q=tips;complete=!100";
                const string operationName = "Future Tips";

                var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                    endpoint,
                    operationName,
                    "tips"
                );

                return result ?? new List<TipsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for GetFutureTips");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFutureTips");
                throw;
            }
        }
    }
}