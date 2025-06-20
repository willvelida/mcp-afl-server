using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class GameTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GameTools> _logger;

        public GameTools(HttpClient httpClient, ILogger<GameTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Gets result from a played game")]
        public async Task<List<GameResponse>> GetGameResult(
            [Description("The ID of the game")] int gameId)
        {
            // Input validation
            if (gameId <= 0)
            {
                _logger.LogWarning("Invalid game ID parameter: {GameId}", gameId);
                return new List<GameResponse>();
            }

            var endpoint = $"?q=games;game={gameId}";
            _logger.LogInformation("Fetching data for Game: {GameId}", gameId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<GameResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("games", out var gamesProperty))
                {
                    _logger.LogWarning("No 'games' property found in API response for Game: {GameId}", gameId);
                    return new List<GameResponse>();
                }

                var gameResponses = JsonSerializer.Deserialize<List<GameResponse>>(
                    gamesProperty.GetRawText());

                if (gameResponses == null || !gameResponses.Any())
                {
                    _logger.LogInformation("No game found for Game ID: {GameId}", gameId);
                    return new List<GameResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} game result(s) for Game: {GameId}", 
                    gameResponses.Count, gameId);
                
                return gameResponses;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching game result for Game: {GameId}", gameId);
                return new List<GameResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching game result for Game: {GameId}", gameId);
                return new List<GameResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for game result Game: {GameId}", gameId);
                return new List<GameResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching game result for Game: {GameId}", gameId);
                return new List<GameResponse>();
            }
        }

        [McpServerTool, Description("Get the results from a round of a particular year")]
        public async Task<List<GameResponse>> GetRoundResultsByYear(
            [Description("The year of the round")] int year,
            [Description("The round number")] int round)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<GameResponse>();
            }

            if (!IsValidRound(round))
            {
                _logger.LogWarning("Invalid round parameter: {Round}", round);
                return new List<GameResponse>();
            }

            var endpoint = $"?q=games;year={year};round={round}";
            _logger.LogInformation("Fetching games for Year: {Year}, Round: {Round}", year, round);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<GameResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("games", out var gamesProperty))
                {
                    _logger.LogWarning("No 'games' property found in API response for Year: {Year}, Round: {Round}", 
                        year, round);
                    return new List<GameResponse>();
                }

                var gameResponses = JsonSerializer.Deserialize<List<GameResponse>>(
                    gamesProperty.GetRawText());

                if (gameResponses == null || !gameResponses.Any())
                {
                    _logger.LogInformation("No games found for Year: {Year}, Round: {Round}", year, round);
                    return new List<GameResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} games for Year: {Year}, Round: {Round}", 
                    gameResponses.Count, year, round);
                
                return gameResponses;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching games for Year: {Year}, Round: {Round}", year, round);
                return new List<GameResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching games for Year: {Year}, Round: {Round}", year, round);
                return new List<GameResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for games Year: {Year}, Round: {Round}", year, round);
                return new List<GameResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching games for Year: {Year}, Round: {Round}", year, round);
                return new List<GameResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
        private static bool IsValidRound(int round) => round >= 1 && round <= 30;
    }
}
