using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class TeamTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TeamTools> _logger;

        public TeamTools(HttpClient httpClient, ILogger<TeamTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Gets information for a AFL team")]
        public async Task<List<TeamResponse>> GetTeamInfo(
            [Description("The ID of the team")] int teamId)
        {
            // Input validation
            if (teamId <= 0)
            {
                _logger.LogWarning("Invalid team ID parameter: {TeamId}", teamId);
                return new List<TeamResponse>();
            }

            var endpoint = $"?q=teams;team={teamId}";
            _logger.LogInformation("Fetching team information for Team ID: {TeamId}", teamId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<TeamResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("teams", out var teamsProperty))
                {
                    _logger.LogWarning("No 'teams' property found in API response for Team ID: {TeamId}", teamId);
                    return new List<TeamResponse>();
                }

                var teamResponses = JsonSerializer.Deserialize<List<TeamResponse>>(
                    teamsProperty.GetRawText());

                if (teamResponses == null || !teamResponses.Any())
                {
                    _logger.LogInformation("No team found with ID: {TeamId}", teamId);
                    return new List<TeamResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} team(s) for ID: {TeamId}", 
                    teamResponses.Count, teamId);
                
                return teamResponses;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching team information for Team ID: {TeamId}", teamId);
                return new List<TeamResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching team information for Team ID: {TeamId}", teamId);
                return new List<TeamResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for team information Team ID: {TeamId}", teamId);
                return new List<TeamResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching team information for Team ID: {TeamId}", teamId);
                return new List<TeamResponse>();
            }
        }

        [McpServerTool, Description("Gets a list of teams who played in a particular season")]
        public async Task<List<TeamResponse>> GetTeamsBySeason(
            [Description("The year to get teams for")] int year)
        {
            // Input validation
            if (!IsValidYear(year))
            {
                _logger.LogWarning("Invalid year parameter: {Year}", year);
                return new List<TeamResponse>();
            }

            var endpoint = $"?q=teams;year={year}";
            _logger.LogInformation("Fetching teams for Year: {Year}", year);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<TeamResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("teams", out var teamsProperty))
                {
                    _logger.LogWarning("No 'teams' property found in API response for Year: {Year}", year);
                    return new List<TeamResponse>();
                }

                var teamResponses = JsonSerializer.Deserialize<List<TeamResponse>>(
                    teamsProperty.GetRawText());

                if (teamResponses == null || !teamResponses.Any())
                {
                    _logger.LogInformation("No teams found for Year: {Year}", year);
                    return new List<TeamResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} teams for Year: {Year}", 
                    teamResponses.Count, year);
                
                return teamResponses;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching teams for Year: {Year}", year);
                return new List<TeamResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching teams for Year: {Year}", year);
                return new List<TeamResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for teams Year: {Year}", year);
                return new List<TeamResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching teams for Year: {Year}", year);
                return new List<TeamResponse>();
            }
        }

        private static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
    }
}
