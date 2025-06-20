using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class StandingsTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StandingsTools> _logger;

        public StandingsTools(HttpClient httpClient, ILogger<StandingsTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Gets the current standing")]
        public async Task<List<StandingsResponse>> GetCurrentStandings()
        {
            const string endpoint = "?q=standings";
            _logger.LogInformation("Fetching current standings");

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<StandingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("standings", out var standingsProperty))
                {
                    _logger.LogWarning("No 'standings' property found in API response for current standings");
                    return new List<StandingsResponse>();
                }

                var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(
                    standingsProperty.GetRawText());

                if (standingsResponse == null || !standingsResponse.Any())
                {
                    _logger.LogInformation("No current standings found");
                    return new List<StandingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} current standings", standingsResponse.Count);
                return standingsResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching current standings");
                return new List<StandingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching current standings");
                return new List<StandingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for current standings");
                return new List<StandingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching current standings");
                return new List<StandingsResponse>();
            }
        }

        [McpServerTool, Description("Get the standings for a particular round and year")]
        public async Task<List<StandingsResponse>> GetStandingsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the standings")] int year)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<StandingsResponse>();
            }

            if (!IsValidRound(roundNumber)) // ✅ Fixed: was IsValidRound(year)
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<StandingsResponse>();
            }

            var endpoint = $"?q=standings;year={year};round={roundNumber}";
            _logger.LogInformation("Fetching standings for Year: {Year}, Round: {Round}", year, roundNumber);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<StandingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("standings", out var standingsProperty))
                {
                    _logger.LogWarning("No 'standings' property found in API response for Year: {Year}, Round: {Round}", 
                        year, roundNumber);
                    return new List<StandingsResponse>();
                }

                var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(
                    standingsProperty.GetRawText()); // ✅ Fixed: was jsonElement.GetRawText()

                if (standingsResponse == null || !standingsResponse.Any())
                {
                    _logger.LogInformation("No standings found for Year: {Year}, Round: {Round}", year, roundNumber);
                    return new List<StandingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} standings for Year: {Year}, Round: {Round}", 
                    standingsResponse.Count, year, roundNumber);

                return standingsResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching standings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<StandingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching standings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<StandingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for standings Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<StandingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching standings for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<StandingsResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private static bool IsValidRound(int round) => round >= 1 && round <= 30; // ✅ Updated: was 23, now 30 for consistency
    }
}
