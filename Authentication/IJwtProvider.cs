namespace BucketSurvey.Api.Authentication;

public interface IJwtProvider
{
    (string Token, int Expiresin) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> Roles, IEnumerable<string> Permissions);
    string? ValidateToken(string Token);


}
