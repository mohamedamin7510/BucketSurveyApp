using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace BucketSurvey.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _JwtOptions = options.Value;

    public ( string Token , int Expiresin ) GenerateToken
        (ApplicationUser applicationUser, IEnumerable<string> roles , IEnumerable<string> Permissions) 
    {

        Claim [] claims = 
            [
               new Claim (JwtRegisteredClaimNames.Sub , applicationUser.Id),
               new Claim (JwtRegisteredClaimNames.Email , applicationUser.Email!),
               new Claim (JwtRegisteredClaimNames.GivenName, applicationUser.FirstName!),
               new Claim (JwtRegisteredClaimNames.FamilyName, applicationUser.LastName!),
               new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim (nameof(roles),JsonSerializer.Serialize(roles) , JsonClaimValueTypes.JsonArray), 
               new Claim (nameof(Permissions), JsonSerializer.Serialize(Permissions) , JsonClaimValueTypes.JsonArray)
            ];

        var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions!.SymmetricKey));

        var SigningCredentials = new SigningCredentials(key: SymmetricSecurityKey, algorithm: SecurityAlgorithms.HmacSha256);

        var JwtSecurityToken = new JwtSecurityToken(
            issuer: _JwtOptions!.Issuer,
            audience: _JwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_JwtOptions.ExpiryMiniutes),
            signingCredentials: SigningCredentials
            );

        return (Token: new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken) , _JwtOptions.ExpiryMiniutes);
    }

    public string? ValidateToken(string Token)
    {
        var jwtsecurityhandlertoken = new JwtSecurityTokenHandler();
        var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions!.SymmetricKey));
        try
        {
            jwtsecurityhandlertoken.ValidateToken(Token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,

               IssuerSigningKey = SymmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken= (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)!.Value;

        }
        catch
        {
            return null; 
        }

    

    }
}
