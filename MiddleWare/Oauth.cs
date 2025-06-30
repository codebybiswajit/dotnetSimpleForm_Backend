using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Middleware
{
    public class JwtAuthentication
    {
        private readonly string _secretKey = String.Empty;
        private readonly string _issuer = String.Empty;

        private readonly string audience = String.Empty;

        public JwtAuthentication(IConfiguration configuration)
        {
            _secretKey = configuration["JwtBearer:SecretKey"];
            _issuer = configuration["JwtBearer:Issuer"];
            audience = configuration["JwtBearer:Audience"];
        }

      public string Authenticate(string userName, string password, string mongoId)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_secretKey);

    var audiences = audience.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var claims = new List<Claim>
    {
        new Claim("mongoId", mongoId),              
        new Claim(ClaimTypes.Name, userName)        
    };

    foreach (var aud in audiences)
    {
        claims.Add(new Claim("aud", aud));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(2),
        Issuer = _issuer,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
        }
    }
}
