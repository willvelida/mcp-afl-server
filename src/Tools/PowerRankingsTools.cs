using System.ComponentModel;
using mcp_afl_server.Models;
using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class PowerRankingsTools : BaseAFLTool
    {
        public PowerRankingsTools(
            HttpClient httpClient,
            ILogger<PowerRankingsTools> logger,
            IAuthenticationService authenticationService)
            : base(httpClient, logger, authenticationService)
        {
        }

        [McpServerTool, Description("Get Power Ranking by Round and Year")]
        public async Task<List<PowerRankingsResponse>> GetPowerRankingByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings (power rankings available from 2022 onwards)")] int year)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id}  requested power rankings for year {year}, round {roundNumber}");

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("year", year, val => IsValidPowerRankingYear((int)val), "Year must be 2022 or later (power rankings not available before 2022)"),
                    ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
                {
                    return new List<PowerRankingsResponse>();
                }

                var endpoint = $"?q=power;year={year};round={roundNumber}";
                var operationName = $"Power Rankings for Year: {year}, Round: {roundNumber}";

                var result = await ExecuteApiCallAsync<List<PowerRankingsResponse>>(
                    endpoint,
                    operationName,
                    "power"
                );

                return result ?? new List<PowerRankingsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetPowerRankingByRoundAndYear: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPowerRankingByRoundAndYear for year {year}, round {roundNumber}: {ex.Message}");
                throw;
            }
        }

        [McpServerTool, Description("Get Power Ranking by Round, Year, and Model Source")]
        public async Task<List<PowerRankingsResponse>> GetPowerRankingByRoundYearAndSource(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings (power rankings available from 2022 onwards)")] int year,
            [Description("The source ID of the model")] int sourceId)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested power rankings for year {year}, round {roundNumber}, source {sourceId}");

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("year", year, val => IsValidPowerRankingYear((int)val), "Year must be 2022 or later (power rankings not available before 2022)"),
                    ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30"),
                    ("sourceId", sourceId, val => (int)val > 0, "Source ID must be a positive integer")))
                {
                    return new List<PowerRankingsResponse>();
                }

                var endpoint = $"?q=power;year={year};round={roundNumber};source={sourceId}";
                var operationName = $"Power Rankings for Year: {year}, Round: {roundNumber}, Source: {sourceId}";

                var result = await ExecuteApiCallAsync<List<PowerRankingsResponse>>(
                    endpoint,
                    operationName,
                    "power"
                );

                return result ?? new List<PowerRankingsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetPowerRankingByRoundYearAndSource: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPowerRankingByRoundYearAndSource for year {year}, round {roundNumber}, source {sourceId}: {ex.Message}");
                throw;
            }
        }

        [McpServerTool, Description("Get Power Ranking for Team by Round, Year, and Model Source")]
        public async Task<List<PowerRankingsResponse>> GetTeamPowerRankingByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings (power rankings available from 2022 onwards)")] int year,
            [Description("The Team Id")] int teamId)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation($"User {user?.Id} requested team power rankings for year {year}, round {roundNumber}, team {teamId}");

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("year", year, val => IsValidPowerRankingYear((int)val), "Year must be 2022 or later (power rankings not available before 2022)"),
                    ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30"),
                    ("teamId", teamId, val => (int)val > 0, "Team ID must be a positive integer")))
                {
                    return new List<PowerRankingsResponse>();
                }

                var endpoint = $"?q=power;year={year};round={roundNumber};team={teamId}";
                var operationName = $"Team Power Rankings for Year: {year}, Round: {roundNumber}, Team: {teamId}";

                var result = await ExecuteApiCallAsync<List<PowerRankingsResponse>>(
                    endpoint,
                    operationName,
                    "power"
                );

                return result ?? new List<PowerRankingsResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt for GetTeamPowerRankingByRoundAndYear: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetTeamPowerRankingByRoundAndYear for year {year}, round {roundNumber}, team {teamId}: {ex.Message}");
                throw;
            }
        }

        protected static bool IsValidPowerRankingYear(int year) => year >= 2022 && year <= DateTime.Now.Year + 1;
    }
}