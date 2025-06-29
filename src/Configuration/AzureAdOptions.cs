using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcp_afl_server.Configuration
{
    public class AzureAdOptions
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ManagedIdentityClientId { get; set; } = string.Empty;
    }
}
