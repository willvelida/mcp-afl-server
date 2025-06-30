using System.ComponentModel;
using mcp_afl_server.Models;
using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class StandingsTools : BaseAFLTool
    {
        public StandingsTools(
            HttpClient httpClient,
            ILogger<StandingsTools> logger,
            IAuthenticationService authenticationService)
            : base(httpClient, logger, authenticationService)
        {
        }

        [McpServerTool, Description("Gets the current standing")]
        public async Task<List<StandingsResponse>> GetCurrentStandings()
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested current standings");

                const string endpoint = "?q=standings";
                const string operationName = "Current Standings";

                var result = await ExecuteApiCallAsync<List<StandingsResponse>>(
                    endpoint,
                    operationName,
                    "standings"
                );

                return result ?? new List<StandingsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetCurrentStandings: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCurrentStandings: {ex.Message}");
                throw;
            }
        }

        [McpServerTool, Description("Get the standings for a particular round and year")]
        public async Task<List<StandingsResponse>> GetStandingsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the standings")] int year)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested standings for year {year}, round {roundNumber}");

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                    ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
                {
                    return new List<StandingsResponse>();
                }

                var endpoint = $"?q=standings;year={year};round={roundNumber}";
                var operationName = $"Standings for Year: {year}, Round: {roundNumber}";

                var result = await ExecuteApiCallAsync<List<StandingsResponse>>(
                    endpoint,
                    operationName,
                    "standings"
                );

                return result ?? new List<StandingsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetStandingsByRoundAndYear: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetStandingsByRoundAndYear for year {year}, round {roundNumber}: {ex.Message}");
                throw;
            }
        }
    }
}