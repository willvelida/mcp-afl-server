using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class TipsTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TipsTools> _logger;

        public TipsTools(HttpClient httpClient, ILogger<TipsTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Get the tips for a particular round and year")]
        public async Task<List<TipsResponse>> GetTipsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the tips")] int year)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<TipsResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<TipsResponse>();
            }

            var endpoint = $"?q=tips;year={year};round={roundNumber}";
            _logger.LogInformation("Fetching tips for Year: {Year}, Round: {Round}", year, roundNumber);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<TipsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("tips", out var tipsProperty))
                {
                    _logger.LogWarning("No 'tips' property found in API response for Year: {Year}, Round: {Round}", 
                        year, roundNumber);
                    return new List<TipsResponse>();
                }

                var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                    tipsProperty.GetRawText());

                if (tipsResponse == null || !tipsResponse.Any())
                {
                    _logger.LogInformation("No tips found for Year: {Year}, Round: {Round}", year, roundNumber);
                    return new List<TipsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} tips for Year: {Year}, Round: {Round}", 
                    tipsResponse.Count, year, roundNumber);
                
                return tipsResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching tips for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<TipsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching tips for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<TipsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for tips Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<TipsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching tips for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<TipsResponse>();
            }
        }

        [McpServerTool, Description("Get the tips of a particular game")]
        public async Task<List<TipsResponse>> GetTipsByGame(
            [Description("The ID of the game")] int gameId)
        {
            // Input validation
            if (gameId <= 0)
            {
                _logger.LogWarning("Invalid game ID parameter: {GameId}", gameId);
                return new List<TipsResponse>();
            }

            var endpoint = $"?q=tips;game={gameId}";
            _logger.LogInformation("Fetching tips for Game ID: {GameId}", gameId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<TipsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("tips", out var tipsProperty))
                {
                    _logger.LogWarning("No 'tips' property found in API response for Game ID: {GameId}", gameId);
                    return new List<TipsResponse>();
                }

                var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                    tipsProperty.GetRawText());

                if (tipsResponse == null || !tipsResponse.Any())
                {
                    _logger.LogInformation("No tips found for Game ID: {GameId}", gameId);
                    return new List<TipsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} tips for Game ID: {GameId}", 
                    tipsResponse.Count, gameId);
                
                return tipsResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching tips for Game ID: {GameId}", gameId);
                return new List<TipsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching tips for Game ID: {GameId}", gameId);
                return new List<TipsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for tips Game ID: {GameId}", gameId);
                return new List<TipsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching tips for Game ID: {GameId}", gameId);
                return new List<TipsResponse>();
            }
        }

        [McpServerTool, Description("Get the tips for current and future games")]
        public async Task<List<TipsResponse>> GetFutureTips()
        {
            const string endpoint = "?q=tips;complete=!100";
            _logger.LogInformation("Fetching future tips");

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<TipsResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("tips", out var tipsProperty))
                {
                    _logger.LogWarning("No 'tips' property found in API response for future tips");
                    return new List<TipsResponse>();
                }

                var tipsResponse = JsonSerializer.Deserialize<List<TipsResponse>>(
                    tipsProperty.GetRawText());

                if (tipsResponse == null || !tipsResponse.Any())
                {
                    _logger.LogInformation("No future tips found");
                    return new List<TipsResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} future tips", tipsResponse.Count);
                
                return tipsResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching future tips");
                return new List<TipsResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching future tips");
                return new List<TipsResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for future tips");
                return new List<TipsResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching future tips");
                return new List<TipsResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private static bool IsValidRound(int round) => round >= 1 && round <= 30;
    }
}
