
namespace BucketSurvey.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public bool IsDisabled { get; set; } = false; 
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
