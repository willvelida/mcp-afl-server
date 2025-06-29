using Azure.Core;
using Azure.Identity;
using mcp_afl_server.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcp_afl_server.Utilities
{
    /// <summary>
    /// Utility class for creating Microsoft Graph clients using On-Behalf-Of (OBO) flow.
    /// </summary>
    public static class GraphClientHelper
    {
        /// <summary>
        /// Creates a GraphServiceClient using On-Behalf-Of authentication flow.
        /// </summary>
        /// <param name="accessToken">The access token to use for OBO flow.</param>
        /// <param name="azureAdOptions">Azure AD configuration options.</param>
        /// <returns>A configured GraphServiceClient instance.</returns>
        /// <exception cref="ArgumentException">Thrown when access token is null or empty.</exception>
        public static GraphServiceClient CreateGraphClient(string accessToken, AzureAdOptions azureAdOptions)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }

            var credential = CreateOnBehalfOfCredential(accessToken, azureAdOptions);
            return new GraphServiceClient(credential);
        }

        private static OnBehalfOfCredential CreateOnBehalfOfCredential(string accessToken, AzureAdOptions azureAdOptions)
        {
            var managedIdentity = new ManagedIdentityCredential(azureAdOptions.ManagedIdentityClientId);

            return new OnBehalfOfCredential(
                tenantId: azureAdOptions.TenantId,
                clientId: azureAdOptions.ClientId,
                clientAssertionCallback: async (cancellationToken) => {
                    var tokenResult = await managedIdentity.GetTokenAsync(new TokenRequestContext(["api://AzureADTokenExchange"]));
                    return tokenResult.Token;
                },
                userAssertion: accessToken);
        }
    }
}
