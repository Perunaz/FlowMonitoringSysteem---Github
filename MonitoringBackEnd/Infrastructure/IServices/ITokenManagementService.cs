using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IServices
{
    public interface ITokenManagementService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetTokensAsync();
        Task<string> RefreshAccessTokenAsync();
    }
}
