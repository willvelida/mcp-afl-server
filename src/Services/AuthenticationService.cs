using mcp_afl_server.Configuration;
using mcp_afl_server.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;

namespace mcp_afl_server.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AzureAdOptions _azureAdOptions;

        public AuthenticationService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AzureAdOptions> azureAdOptions)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _azureAdOptions = azureAdOptions?.Value ?? throw new ArgumentNullException(nameof(azureAdOptions));
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var graphClient = CreateGraphClient();
            var user = await graphClient.Me.GetAsync();
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Unable to retrieve user information");
            }

            return user;
        }

        public bool IsAuthenticated()
        {
            try
            {
                var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
                return !string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private Microsoft.Graph.GraphServiceClient CreateGraphClient()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                throw new UnauthorizedAccessException("No authorization header found in the request");
            }

            // Extract the token from "Bearer <token>"
            var token = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader.Substring("Bearer ".Length).Trim()
                : authorizationHeader;

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid authorization token format");
            }

            return GraphClientHelper.CreateGraphClient(token, _azureAdOptions);
        }
    }
}