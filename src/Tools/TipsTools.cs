using System.ComponentModel;
using mcp_afl_server.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace mcp_afl_server.Tools
{
    [McpServerToolType]
    public class TipsTools : BaseAFLTool
    {
        public TipsTools(HttpClient httpClient, ILogger<TipsTools> logger)
            : base(httpClient, logger)
        {
        }

        [McpServerTool, Description("Get the tips for a particular round and year")]
        public async Task<List<TipsResponse>> GetTipsByRoundAndYear(
            [Description("The round that has been played")] int roundNumber,
            [Description("The year of the tips")] int year)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("year", year, val => IsValidYear((int)val), "Year must be between 1897 and current year + 1"),
                ("roundNumber", roundNumber, val => IsValidRound((int)val), "Round must be between 1 and 30")))
            {
                return new List<TipsResponse>();
            }

            var endpoint = $"?q=tips;year={year};round={roundNumber}";
            var operationName = $"Tips for Year: {year}, Round: {roundNumber}";

            var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                endpoint,
                operationName,
                "tips"
            );

            return result ?? new List<TipsResponse>();
        }

        [McpServerTool, Description("Get the tips of a particular game")]
        public async Task<List<TipsResponse>> GetTipsByGame(
            [Description("The ID of the game")] int gameId)
        {
            // Validate parameters using base class method
            if (!ValidateParameters(
                ("gameId", gameId, val => IsValidId((int)val), "Game ID must be a positive integer")))
            {
                return new List<TipsResponse>();
            }

            var endpoint = $"?q=tips;game={gameId}";
            var operationName = $"Tips for Game ID: {gameId}";

            var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                endpoint,
                operationName,
                "tips"
            );

            return result ?? new List<TipsResponse>();
        }

        [McpServerTool, Description("Get the tips for current and future games")]
        public async Task<List<TipsResponse>> GetFutureTips()
        {
            const string endpoint = "?q=tips;complete=!100";
            const string operationName = "Future Tips";

            var result = await ExecuteApiCallAsync<List<TipsResponse>>(
                endpoint,
                operationName,
                "tips"
            );

            return result ?? new List<TipsResponse>();
        }
    }
}
