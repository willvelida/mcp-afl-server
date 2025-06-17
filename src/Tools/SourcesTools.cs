using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class SourcesTools
    {
        [McpServerTool, Description("Gets a list of sources")]
        public static async Task<string> GetSources(HttpClient httpClient)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>("?q=sources");
            var sourcesResponse = JsonSerializer.Deserialize<List<SourcesResponse>>(
                response.GetProperty("sources").GetRawText());

            if (!sourcesResponse.Any() || sourcesResponse == null)
            {
                return "No sources found";
            }

            return string.Join("\n--\n", sourcesResponse.Select(source => FormatSourceResponse(source)));
        }

        [McpServerTool, Description("Gets a source by ID")]
        public static async Task<string> GetSourceById(
            HttpClient httpClient,
            [Description("The ID of the source")] string sourceId)
        {
            var response = await httpClient.GetFromJsonAsync<JsonElement>($"?q=sources;source={sourceId}");
            var sourcesResponse = JsonSerializer.Deserialize<List<SourcesResponse>>(
                response.GetProperty("sources").GetRawText());

            if (!sourcesResponse.Any() || sourcesResponse == null)
            {
                return "No source found with this ID";
            }

            return string.Join("\n--\n", sourcesResponse.Select(source => FormatSourceResponse(source)));
        }

        private static string FormatSourceResponse(SourcesResponse source)
        {

            return $"""
                Name: {source.Name}
                URL: {source.Url}
                ID: {source.Id}
                Icon: {source.Icon}
                """;

        }
    }
}
