using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcp_afl_server.Services
{
    public interface IAuthenticationService
    {
        Task<User> GetCurrentUserAsync();
        bool IsAuthenticated();
    }
}
