using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class SourcesTools
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SourcesTools> _logger;

        public SourcesTools(HttpClient httpClient, ILogger<SourcesTools> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [McpServerTool, Description("Gets a list of sources")]
        public async Task<List<SourcesResponse>> GetSources()
        {
            const string endpoint = "?q=sources";
            _logger.LogInformation("Fetching all sources");

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<SourcesResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("sources", out var sourcesProperty))
                {
                    _logger.LogWarning("No 'sources' property found in API response");
                    return new List<SourcesResponse>();
                }

                var sourcesResponse = JsonSerializer.Deserialize<List<SourcesResponse>>(
                    sourcesProperty.GetRawText());

                if (sourcesResponse == null || !sourcesResponse.Any())
                {
                    _logger.LogInformation("No sources found");
                    return new List<SourcesResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} sources", sourcesResponse.Count);
                return sourcesResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching sources");
                return new List<SourcesResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching sources");
                return new List<SourcesResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for sources");
                return new List<SourcesResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching sources");
                return new List<SourcesResponse>();
            }
        }

        [McpServerTool, Description("Gets a source by ID")]
        public async Task<List<SourcesResponse>> GetSourceById(
            [Description("The ID of the source")] string sourceId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(sourceId))
            {
                _logger.LogWarning("Invalid source ID parameter: source ID cannot be null or empty");
                return new List<SourcesResponse>();
            }

            var endpoint = $"?q=sources;source={Uri.EscapeDataString(sourceId)}";
            _logger.LogInformation("Fetching source by ID: {SourceId}", sourceId);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}", 
                        response.StatusCode, endpoint);
                    return new List<SourcesResponse>();
                }

                var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonElement.TryGetProperty("sources", out var sourcesProperty))
                {
                    _logger.LogWarning("No 'sources' property found in API response for Source ID: {SourceId}", sourceId);
                    return new List<SourcesResponse>();
                }

                var sourcesResponse = JsonSerializer.Deserialize<List<SourcesResponse>>(
                    sourcesProperty.GetRawText());

                if (sourcesResponse == null || !sourcesResponse.Any())
                {
                    _logger.LogInformation("No source found with ID: {SourceId}", sourceId);
                    return new List<SourcesResponse>();
                }

                _logger.LogInformation("Successfully retrieved {Count} source(s) for ID: {SourceId}", 
                    sourcesResponse.Count, sourceId);
                
                return sourcesResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching source by ID: {SourceId}", sourceId);
                return new List<SourcesResponse>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching source by ID: {SourceId}", sourceId);
                return new List<SourcesResponse>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error for source ID: {SourceId}", sourceId);
                return new List<SourcesResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching source by ID: {SourceId}", sourceId);
                return new List<SourcesResponse>();
            }
        }
    }
}
