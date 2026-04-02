namespace BucketSurvey.Api.Authentication;

public class JwtOptions
{
    public static string SectionName { get; set; } = "Jwt";
    [Required]
    public string SymmetricKey { get; init; } = string.Empty!;
    [Required]
    public string Issuer { get; init; } = string.Empty!;
    [Required]
    public string Audience { get; init; } = string.Empty!;
    [Range(1, int.MaxValue, ErrorMessage = "ExpiryMiniutes must be in Range ")]
    public int ExpiryMiniutes { get; init; }


}
