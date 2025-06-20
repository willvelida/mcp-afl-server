using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class PowerRankingsTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PowerRankingsTools> _logger;

        public PowerRankingsTools(HttpClient httpClient, ILogger<PowerRankingsTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Get Power Ranking by Round and Year")]
        public async Task<List<PowerRankingsResponse>> GetPowerRankingByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<PowerRankingsResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<PowerRankingsResponse>();
            }

            var endpoint = $"?q=power;year={year};round={roundNumber}";
            _logger.LogInformation("Fetching power rankings for Year: {Year}, Round: {Round}", year, roundNumber);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<PowerRankingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("power", out var powerProperty))
                {
                    _logger.LogWarning("No 'power' property found in API response for Year: {Year}, Round: {Round}", 
                        year, roundNumber);
                    return new List<PowerRankingsResponse>();
                }

                var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                    powerProperty.GetRawText());

                if (powerRankingResponse == null || !powerRankingResponse.Any())
                {
                    _logger.LogInformation("No power rankings found for Year: {Year}, Round: {Round}", year, roundNumber);
                    return new List<PowerRankingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} power rankings for Year: {Year}, Round: {Round}", 
                    powerRankingResponse.Count, year, roundNumber);
                
                return powerRankingResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching power rankings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<PowerRankingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching power rankings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<PowerRankingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for power rankings Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<PowerRankingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching power rankings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<PowerRankingsResponse>();
            }
        }

        [McpServerTool, Description("Get Power Ranking by Round, Year, and Model Source")]
        public async Task<List<PowerRankingsResponse>> GetPowerRankingByRoundYearAndSource(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year,
            [Description("The source ID of the model")] int sourceId)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<PowerRankingsResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<PowerRankingsResponse>();
            }

            if (sourceId <= 0)
            {
                _logger.LogWarning("Invalid source ID parameter: {SourceId}", sourceId);
                return new List<PowerRankingsResponse>();
            }

            var endpoint = $"?q=power;year={year};round={roundNumber};source={sourceId}";
            _logger.LogInformation("Fetching power rankings for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                year, roundNumber, sourceId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<PowerRankingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("power", out var powerProperty))
                {
                    _logger.LogWarning("No 'power' property found in API response for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                        year, roundNumber, sourceId);
                    return new List<PowerRankingsResponse>();
                }

                var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                    powerProperty.GetRawText());

                if (powerRankingResponse == null || !powerRankingResponse.Any())
                {
                    _logger.LogInformation("No power rankings found for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                        year, roundNumber, sourceId);
                    return new List<PowerRankingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} power rankings for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                    powerRankingResponse.Count, year, roundNumber, sourceId);
                
                return powerRankingResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching power rankings for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                    year, roundNumber, sourceId);
                return new List<PowerRankingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching power rankings for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                    year, roundNumber, sourceId);
                return new List<PowerRankingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for power rankings Year: {Year}, Round: {Round}, Source: {SourceId}", 
                    year, roundNumber, sourceId);
                return new List<PowerRankingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching power rankings for Year: {Year}, Round: {Round}, Source: {SourceId}", 
                    year, roundNumber, sourceId);
                return new List<PowerRankingsResponse>();
            }
        }

        [McpServerTool, Description("Get Power Ranking for Team by Round, Year, and Model Source")]
        public async Task<List<PowerRankingsResponse>> GetTeamPowerRankingByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the rankings")] int year,
            [Description("The Team Id")] int teamId)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<PowerRankingsResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<PowerRankingsResponse>();
            }

            if (teamId <= 0)
            {
                _logger.LogWarning("Invalid team ID parameter: {TeamId}", teamId);
                return new List<PowerRankingsResponse>();
            }

            var endpoint = $"?q=power;year={year};round={roundNumber};team={teamId}";
            _logger.LogInformation("Fetching team power rankings for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                year, roundNumber, teamId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<PowerRankingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("power", out var powerProperty))
                {
                    _logger.LogWarning("No 'power' property found in API response for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                        year, roundNumber, teamId);
                    return new List<PowerRankingsResponse>();
                }

                var powerRankingResponse = JsonSerializer.Deserialize<List<PowerRankingsResponse>>(
                    powerProperty.GetRawText());

                if (powerRankingResponse == null || !powerRankingResponse.Any())
                {
                    _logger.LogInformation("No team power rankings found for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                        year, roundNumber, teamId);
                    return new List<PowerRankingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} team power rankings for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                    powerRankingResponse.Count, year, roundNumber, teamId);
                
                return powerRankingResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching team power rankings for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                    year, roundNumber, teamId);
                return new List<PowerRankingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching team power rankings for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                    year, roundNumber, teamId);
                return new List<PowerRankingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for team power rankings Year: {Year}, Round: {Round}, Team: {TeamId}", 
                    year, roundNumber, teamId);
                return new List<PowerRankingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching team power rankings for Year: {Year}, Round: {Round}, Team: {TeamId}", 
                    year, roundNumber, teamId);
                return new List<PowerRankingsResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private static bool IsValidRound(int round) => round >= 1 && round <= 30;
    }
}
