using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username, string role, int id, List<string> permissions
);
        string GenerateRefreshToken();
    }
}
