using System.ComponentModel;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class LadderTools : BaseAFLTool
    {
        public LadderTools(HttpClient httpClient, ILogger<LadderTools> logger)
            : base(httpClient, logger)
        {
        }

        [McpServerTool, Description("Get the projected ladder for a particular round and year")]
        public async Task<List<LadderResponse>> GetProjectedLadderByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
            {
                return new List<LadderResponse>();
            }

            var endpoint = $"?q=ladder;year={year};round={roundNumber}";
            var operationName = $"Projected Ladder for Year: {year}, Round: {roundNumber}";

            var result = await ExecuteApiCallAsync<List<LadderResponse>>(
                endpoint,
                operationName,
                "ladder"
            );

            return result ?? new List<LadderResponse>();
        }

        [McpServerTool, Description("Get the projected ladder for a particular round and year by source")]
        public async Task<List<LadderResponse>> GetProjectedLadderByRoundAndYearBySource(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the ladder")] int year,
            [Description("The source of the ladder")] string source)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30"),
                ("source", source, val => IsValidString((string)val), "Source cannot be null or empty")))
            {
                return new List<LadderResponse>();
            }

            var endpoint = $"?q=ladder;year={year};round={roundNumber};source={source}";

            var operationName = $"Projected Ladder for Year: {year}, Round: {roundNumber}, Source: {source}";

            var result = await ExecuteApiCallAsync<List<LadderResponse>>(
                endpoint,
                operationName,
                "ladder"
            );

            return result ?? new List<LadderResponse>();
        }
    }
}
