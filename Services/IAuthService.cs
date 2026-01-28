using BucketSurvey.Api.Contract.Authentication;
using OneOf;

namespace BucketSurvey.Api.Services;

public interface IAuthService
{
    public Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> GetRefreshTokenAsync(string token , string refreshtoken, CancellationToken cancellationToken = default);
    public Task<OneOf<bool,Error>> RevokeRefreshTokenAsync(string token , string refreshtoken, CancellationToken cancellationToken = default);
}
