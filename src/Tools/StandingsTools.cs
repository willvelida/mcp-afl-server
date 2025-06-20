using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class StandingsTools : BaseAFLTool
    {
        public StandingsTools(HttpClient httpClient, ILogger<StandingsTools> logger)
            : base(httpClient, logger)
        {
        }

        [McpServerTool, Description("Gets the current standing")]
        public async Task<List<StandingsResponse>> GetCurrentStandings()
        {
            const string endpoint = "?q=standings";
            const string operationName = "Current Standings";

            var result = await ExecuteApiCallAsync<List<StandingsResponse>>(
                endpoint,
                operationName,
                "standings"
            );

            return result ?? new List<StandingsResponse>();
        }

        [McpServerTool, Description("Get the standings for a particular round and year")]
        public async Task<List<StandingsResponse>> GetStandingsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the standings")] int year)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
            {
                return new List<StandingsResponse>();
            }

            var endpoint = $"?q=standings;year={year};round={roundNumber}";
            var operationName = $"Standings for Year: {year}, Round: {roundNumber}";

            var result = await ExecuteApiCallAsync<List<StandingsResponse>>(
                endpoint,
                operationName,
                "standings"
            );

            return result ?? new List<StandingsResponse>();
        }
    }
}
