using AuthorizationServer.Protos;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationServer
{
    public static class JwtAuthenticationManager
    {
        public const string JWT_TOKEN_KEY = "oX1V8vR/nwnFV5PArE+SG+gb3rVvqRiJ37RnYDCJ3Os=";
        private const int JWT_TOKEN_LIFE_MINUTES = 30;
        public static AuthenticationResponse Authenticate(AuthenticationRequest authenticationRequest)
        {
            if (authenticationRequest.UserName != "admin" || authenticationRequest.Password != "admin")
            {
                return null;
            }

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(JWT_TOKEN_KEY);

            var tokenExpiryDateTime = DateTime.Now.AddMinutes(JWT_TOKEN_LIFE_MINUTES);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim> 
                {
                    new Claim("username", authenticationRequest.UserName),
                    new Claim(ClaimTypes.Role, "Admin"),
                }),
                Expires = tokenExpiryDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                AccessToken = token,
                ExpiresIn = (int)tokenExpiryDateTime.Subtract(DateTime.Now).TotalSeconds,
            };
        }
    }
}
