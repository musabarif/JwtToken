using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using JwtToken.Models;
using JwtToken.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;

        public LoginController(IConfiguration configuration, IUserRefreshTokenRepository userRefreshTokenRepository)
        {
            this.configuration = configuration;
            _userRefreshTokenRepository = userRefreshTokenRepository;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.ID.Equals(configuration["Jwt:User:UserID"]) && user.Username.Equals(configuration["Jwt:User:Username"]) && user.Password.Equals(configuration["Jwt:User:Password"]))
            {
                JwtTokenModel jwtTokenModel = GenerateToken(user);
                _userRefreshTokenRepository.SaveUpdateRefreshToken(new UserRefreshToken { Username = user.Username, RefreshToken = jwtTokenModel.RefreshToken });
                return Ok(jwtTokenModel);
            }
            else
            {
                return BadRequest("Invalid Username and Password");
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public IActionResult RefreshToken(JwtTokenModel jwtTokenModel)
        {
            if (jwtTokenModel == null) return BadRequest("Invalid jwtTokenModel");

            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            IPrincipal principal = handler.ValidateToken(jwtTokenModel.Token, GetValidationParameter(), out validatedToken);

            var username = principal.Identity.Name;

            if (_userRefreshTokenRepository.CheckIfRefreshTokenIsValid(username, jwtTokenModel.RefreshToken))
            {
                var newjwtTokenModel = GenerateToken(new Models.User { Username = username });
                _userRefreshTokenRepository.SaveUpdateRefreshToken(new UserRefreshToken { Username = username, RefreshToken = newjwtTokenModel.RefreshToken });

                return Ok(newjwtTokenModel);
            }

            return BadRequest("Invalid RefreshToken");


        }

        private TokenValidationParameters GetValidationParameter()
        {
            var securityKey = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Issuer"],
                ClockSkew = TimeSpan.Zero
            };
        }

        private JwtTokenModel GenerateToken(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Email,user.Username)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: credentials
                );

            var _refreshToken = new RefreshTokenGenerator().GetRefreshToken(32);
            var _jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new JwtTokenModel { Token = _jwtToken, RefreshToken = _refreshToken };
        }
    }
}