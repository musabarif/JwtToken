using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtToken.Models
{
    public class JwtTokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
