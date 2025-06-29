using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class SourcesTools : BaseAFLTool
    {
        public SourcesTools(HttpClient httpClient, ILogger<SourcesTools> logger)
            : base(httpClient, logger)
        {
        }

        [McpServerTool, Description("Gets a list of sources")]
        public async Task<List<SourcesResponse>> GetSources()
        {
            const string endpoint = "?q=sources";
            const string operationName = "All Sources";

            var result = await ExecuteApiCallAsync<List<SourcesResponse>>(
                endpoint,
                operationName,
                "sources"
            );

            return result ?? new List<SourcesResponse>();
        }

        [McpServerTool, Description("Gets a source by ID")]
        public async Task<List<SourcesResponse>> GetSourceById(
            [Description("The ID of the source")] string sourceId)
        {
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
    }
}
