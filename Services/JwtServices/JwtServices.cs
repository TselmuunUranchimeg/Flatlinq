using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Flatlinq.Services;

public class JwtServices : IJwtServices
{
    private readonly IConfiguration _configuration;
    public JwtServices(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(User user, bool isAccessToken, string role)
    {
        DateTime issuedTime = DateTime.Now;
        DateTime expires = issuedTime.Add(isAccessToken ? TimeSpan.FromMinutes(15) : TimeSpan.FromDays(7));
        Claim[] claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, user.Id)
        };
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedTime,
            Expires = expires,
            Issuer = _configuration["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")!)
                ),
                SecurityAlgorithms.HmacSha256
            )
        };
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GetIdFromToken(string tokenString)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        tokenHandler.ValidateToken(tokenString[7..], new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            ),
            ValidIssuer = _configuration["Jwt:Issuer"],
            RequireExpirationTime = true,
            ValidAlgorithms = new string[] {"HS256"},
            ValidateAudience = false
        }, out SecurityToken validatedToken);
        JwtSecurityToken token = (JwtSecurityToken)validatedToken;
        return token.Claims.First(v => v.Type == "nameid").Value;
    }
}