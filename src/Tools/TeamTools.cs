using System.ComponentModel;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class TeamTools : BaseAFLTool
    {
        public TeamTools(HttpClient httpClient, ILogger<TeamTools> logger)
            : base(httpClient, logger)
        {
        }

        [McpServerTool, Description("Gets information for a AFL team")]
        public async Task<List<TeamResponse>> GetTeamInfo(
            [Description("The ID of the team")] int teamId)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("teamId", teamId, val => IsValidId((int)val), "Team ID must be a positive integer")))
            {
                return new List<TeamResponse>();
            }

            var endpoint = $"?q=teams;team={teamId}";
            var operationName = $"Team Info for Team ID: {teamId}";

            var result = await ExecuteApiCallAsync<List<TeamResponse>>(
                endpoint,
                operationName,
                "teams"
            );

            return result ?? new List<TeamResponse>();
        }

        [McpServerTool, Description("Gets a list of teams who played in a particular season")]
        public async Task<List<TeamResponse>> GetTeamsBySeason(
            [Description("The year to get teams for")] int year)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1")))
            {
                return new List<TeamResponse>();
            }

            var endpoint = $"?q=teams;year={year}";
            var operationName = $"Teams for Year: {year}";

            var result = await ExecuteApiCallAsync<List<TeamResponse>>(
                endpoint,
                operationName,
                "teams"
            );

            return result ?? new List<TeamResponse>();
        }
    }
}
