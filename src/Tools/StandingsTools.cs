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
            _logger.LogInformation("Fetching current standings");

            try
            {
                var response = await _httpClient.GetAsync("?q=standings");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API request failed. StatusCode: {response.StatusCode}");
                    return new List<StandingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("standings", out var standingsProperty))
                {
                    _logger.LogWarning("No 'standings' property found in API response for current season");
                    return new List<StandingsResponse>();
                }

                var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(standingsProperty.GetRawText());

                if (standingsResponse == null || !standingsResponse.Any())
                {
                    _logger.LogInformation("No current standing found");
                    return new List<StandingsResponse>();
                }

                _logger.LogInformation("Successfully retrieved standings for current season");
                return standingsResponse;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching current standing");
                return new List<StandingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching current standings");
                return new List<StandingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsisng error for current standings");
                return new List<StandingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving standings");
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
                _logger.LogWarning($"Invalid year: {year}");
                return new List<StandingsResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning($"Invalid round parameter: {roundNumber}");
                return new List<StandingsResponse>();
            }

            _logger.LogInformation($"Fetching standings for Year: {year}, Round: {roundNumber}");


            try
            {
                var response = await _httpClient.GetAsync($"?q=standings;year={year};round={roundNumber}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API request failed. StatusCode: {response.StatusCode}");
                    return new List<StandingsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("standings", out var standingsProperty))
                {
                    _logger.LogWarning("No 'standings' property found in API response for current season");
                    return new List<StandingsResponse>();
                }

                var standingsResponse = JsonSerializer.Deserialize<List<StandingsResponse>>(standingsProperty.GetRawText());

                if (standingsResponse == null || !standingsResponse.Any())
                {
                    _logger.LogInformation($"No standings found for Year: {year}, Round: {roundNumber}");
                    return new List<StandingsResponse>();
                }

                _logger.LogInformation($"Successfully retrieved standings for Year: {year}, Round: {roundNumber}");

                return standingsResponse;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Network error fetching standings after Round {roundNumber}, Year {year}");
                return new List<StandingsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, $"Timeout fetching standings after Round {roundNumber}, Year {year}");
                return new List<StandingsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON parsisng error for standings after Round {roundNumber}, Year {year}");
                return new List<StandingsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving standings after Round {roundNumber}, Year {year}");
                return new List<StandingsResponse>();
            }
        }

        private bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private bool IsValidRound(int round) => round >= 1 && round <= 23;
    }
}
