using System.ComponentModel;
using mcp_afl_server.Models;
using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class SourcesTools : BaseAFLTool
    {
        public SourcesTools(
            HttpClient httpClient,
            ILogger<SourcesTools> logger,
            IAuthenticationService authenticationService)
            : base(httpClient, logger, authenticationService)
        {
        }

        [McpServerTool, Description("Gets a list of sources")]
        public async Task<List<SourcesResponse>> GetSources()
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation("User {UserId} requested all sources", user?.Id);

                const string endpoint = "?q=sources";
                const string operationName = "All Sources";

                var result = await ExecuteApiCallAsync<List<SourcesResponse>>(
                    endpoint,
                    operationName,
                    "sources"
                );

                return result ?? new List<SourcesResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for GetSources");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSources");
                throw;
            }
        }

        [McpServerTool, Description("Gets a source by ID")]
        public async Task<List<SourcesResponse>> GetSourceById(
            [Description("The ID of the source")] string sourceId)
        {
            try
            {
                // Authenticate user and get safe identifier for logging
                var user = await GetCurrentUserAsync();
                _logger.LogInformation("User {UserId} requested source by ID {SourceId}", user?.Id, sourceId);

                // Validate parameters using base class method
                if (!ValidateParameters(
                    ("sourceId", sourceId, val => IsValidString((string)val), "Source ID cannot be null or empty")))
                {
                    return new List<SourcesResponse>();
                }

                var endpoint = $"?q=sources;source={Uri.EscapeDataString(sourceId)}";
                var operationName = $"Source by ID: {sourceId}";

                var result = await ExecuteApiCallAsync<List<SourcesResponse>>(
                    endpoint,
                    operationName,
                    "sources"
                );

                return result ?? new List<SourcesResponse>();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for GetSourceById");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSourceById for source {SourceId}", sourceId);
                throw;
            }
        }
    }
}