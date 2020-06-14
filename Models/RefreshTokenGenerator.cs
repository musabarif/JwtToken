using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JwtToken.Models
{
    public class RefreshTokenGenerator
    {
        public string GetRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetRefreshToken(int size=32)
        {
            var refToken = new byte[size];

            using (var rg= RandomNumberGenerator.Create())
            {
                rg.GetBytes(refToken);

                return Convert.ToBase64String(refToken);
            }
        }
    }
}
