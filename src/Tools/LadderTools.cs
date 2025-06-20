using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class LadderTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LadderTools> _logger;

        public LadderTools(HttpClient httpClient, ILogger<LadderTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Get the projected ladder for a particular round and year")]
        public async Task<List<LadderResponse>> GetProjectedLadderByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<LadderResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<LadderResponse>();
            }

            var endpoint = $"?q=ladder;year={year};round={roundNumber}";
            _logger.LogInformation("Fetching projected ladder for Year: {Year}, Round: {Round}", year, roundNumber);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<LadderResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("ladder", out var ladderProperty))
                {
                    _logger.LogWarning("No 'ladder' property found in API response for Year: {Year}, Round: {Round}", 
                        year, roundNumber);
                    return new List<LadderResponse>();
                }

                var ladderResponse = JsonSerializer.Deserialize<List<LadderResponse>>(
                    ladderProperty.GetRawText());

                if (ladderResponse == null || !ladderResponse.Any())
                {
                    _logger.LogInformation("No projected ladder found for Year: {Year}, Round: {Round}", year, roundNumber);
                    return new List<LadderResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} ladder entries for Year: {Year}, Round: {Round}", 
                    ladderResponse.Count, year, roundNumber);
                
                return ladderResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching projected ladder for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<LadderResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching projected ladder for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<LadderResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for projected ladder Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<LadderResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching projected ladder for Year: {Year}, Round: {Round}", year, roundNumber);
                return new List<LadderResponse>();
            }
        }

        [McpServerTool, Description("Get the projected ladder for a particular round and year by source")]
        public async Task<List<LadderResponse>> GetProjectedLadderByRoundAndYearBySource(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year,
            [Description("The source of the ladder")] string source)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<LadderResponse>();
            }

            if (!IsValidRound(roundNumber))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", roundNumber);
                return new List<LadderResponse>();
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                _logger.LogWarning("Invalid source parameter: source cannot be null or empty");
                return new List<LadderResponse>();
            }

            var endpoint = $"?q=ladder;year={year};round={roundNumber};source={Uri.EscapeDataString(source)}";
            _logger.LogInformation("Fetching projected ladder for Year: {Year}, Round: {Round}, Source: {Source}", 
                year, roundNumber, source);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<LadderResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("ladder", out var ladderProperty))
                {
                    _logger.LogWarning("No 'ladder' property found in API response for Year: {Year}, Round: {Round}, Source: {Source}", 
                        year, roundNumber, source);
                    return new List<LadderResponse>();
                }

                var ladderResponse = JsonSerializer.Deserialize<List<LadderResponse>>(
                    ladderProperty.GetRawText());

                if (ladderResponse == null || !ladderResponse.Any())
                {
                    _logger.LogInformation("No projected ladder found for Year: {Year}, Round: {Round}, Source: {Source}", 
                        year, roundNumber, source);
                    return new List<LadderResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} ladder entries for Year: {Year}, Round: {Round}, Source: {Source}", 
                    ladderResponse.Count, year, roundNumber, source);
                
                return ladderResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching projected ladder for Year: {Year}, Round: {Round}, Source: {Source}", 
                    year, roundNumber, source);
                return new List<LadderResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching projected ladder for Year: {Year}, Round: {Round}, Source: {Source}", 
                    year, roundNumber, source);
                return new List<LadderResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for projected ladder Year: {Year}, Round: {Round}, Source: {Source}", 
                    year, roundNumber, source);
                return new List<LadderResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching projected ladder for Year: {Year}, Round: {Round}, Source: {Source}", 
                    year, roundNumber, source);
                return new List<LadderResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private static bool IsValidRound(int round) => round >= 1 && round <= 30;
    }
}
