using JwtToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtToken.Repository
{
    public class UserRefreshTokenRepository : IUserRefreshTokenRepository
    {
        public Dictionary<string, string> RefreshTokenStore;

        public UserRefreshTokenRepository()
        {
            RefreshTokenStore = new Dictionary<string, string>();
        }
        public bool CheckIfRefreshTokenIsValid(string username, string refreshtoken)
        {
            string refreshTokenInStore = "";

            RefreshTokenStore.TryGetValue(username, out refreshTokenInStore);

            return refreshTokenInStore.Equals(refreshtoken);
        }

        public void SaveUpdateRefreshToken(UserRefreshToken userRefreshToken)
        {
            if(RefreshTokenStore.ContainsKey(userRefreshToken.Username))
            {
                RefreshTokenStore[userRefreshToken.Username] = userRefreshToken.RefreshToken;
            }
            else
            {
                RefreshTokenStore.Add(userRefreshToken.Username, userRefreshToken.RefreshToken);
            }
        }
    }
}
