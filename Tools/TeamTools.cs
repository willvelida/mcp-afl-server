using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
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
            var team = jsonElement.GetProperty("teams").EnumerateArray();

            if (!team.Any())
            {
                return "No team found with this name";
            }

            return string.Join("\n--\n", team.Select(team =>
            {
                return $"""
                    Name: {team.GetProperty("name").GetString()}
                    Debut: {team.GetProperty("debut").GetInt32()}
                """;
            }));
        }

        [McpServerTool, Description("ets a list of teams who played in a particular season")]
        public static async Task<string> GetTeamsBySeason(
            HttpClient httpClient,
            [Description("The year to get teams for")] int year)
        {
            var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>($"?q=teams;year={year}");
            var teams = jsonElement.GetProperty("teams").EnumerateArray();

            if (!teams.Any())
            {
                return "No teams found for this year";
            }

            return string.Join("\n--\n", teams.Select(team =>
            {
                return $"""
                    Name: {team.GetProperty("name").GetString()}
                    Debut: {team.GetProperty("debut").GetInt32()}
                """;
            }));
        }
    }
}
