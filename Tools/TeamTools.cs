using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public static class TeamTools
    {
        [McpServerTool, Description("Gets information for a AFL team")]
        public static async Task<string> GetTeamInfo(
            HttpClient httpClient,
            [Description("The ID of the team")] int teamId)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=teams;team={teamId}");
            var teamResponses = JsonSerializer.Deserialize<List<TeamResponse>>(
                jsonElement.GetProperty("teams").GetRawText());

            if (!teamResponses.Any() || teamResponses == null)
            {
                return "No team found with this name";
            }

            return string.Join("\n--\n", teamResponses.Select(team => FormatTeamResponse(team)));
        }

        [McpServerTool, Description("ets a list of teams who played in a particular season")]
        public static async Task<string> GetTeamsBySeason(
            HttpClient httpClient,
            [Description("The year to get teams for")] int year)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=teams;year={year}");
            var teamResponses = JsonSerializer.Deserialize<List<TeamResponse>>(
                jsonElement.GetProperty("teams").GetRawText());

            if (!teamResponses.Any() || teamResponses == null)
            {
                return $"No teams found for the year {year}";
            }

            return string.Join("\n--\n", teamResponses.Select(team => FormatTeamResponse(team)));
        }

        private static string FormatTeamResponse(TeamResponse team)
        {
            return $"""
                Name: {team.Name}
                Abbreviation: {team.Abbrevation}
                Logo: {team.Logo}
                ID: {team.Id}
                Debut: {team.Debut}
                Retirement: {team.Retirement}
            """;
        }
    }
}
