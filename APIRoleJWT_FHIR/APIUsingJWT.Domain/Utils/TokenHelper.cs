using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Utils
{
    public static class TokenHelper
    {
        public static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashed = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashed);
        }
    }
}
