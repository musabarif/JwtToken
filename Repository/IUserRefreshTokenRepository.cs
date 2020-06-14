using JwtToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtToken.Repository
{
    public interface IUserRefreshTokenRepository
    {
        void SaveUpdateRefreshToken(UserRefreshToken userRefreshToken);

        bool CheckIfRefreshTokenIsValid(string username, string refreshtoken);
    }
}
