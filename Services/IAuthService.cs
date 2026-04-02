using BucketSurvey.Api.Contract.Authentication;
using OneOf;

namespace BucketSurvey.Api.Services;

public interface IAuthService
{
    public Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default);
    public Task<OneOf<bool, Error>> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default);
    public Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    public Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken);
    public Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken);
    public Task<Result> SendResetPassCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken);
    public Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
    public Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest  request , CancellationToken cancellationToken = default);
}
