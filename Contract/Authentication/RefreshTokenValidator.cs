namespace BucketSurvey.Api.Contract.Authentication;

public class RefreshTokenValidator:AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x=>x.token).NotEmpty();
        RuleFor(x => x.refreshToken).NotEmpty();
    }
}
