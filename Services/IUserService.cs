using BucketSurvey.Api.Contract.User;

namespace BucketSurvey.Api.Services;

public interface IUserService
{
    public Task<Result<ProfileResponse>> GetUserAsync(string Id);
    public Task UpdateUserAsync( string Id , ProfileRequest  request );
    public Task<Result> ChangePasswordAsync( string Id , ChangePasswordRequest  request );
    public Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken cancelationToken);
    public Task<Result<UserResponse>> GetUserAsync(string id, CancellationToken cancelationToken);
    public Task<Result<UserResponse>> AddUserAsync(UserRequest request, CancellationToken cancelationToken);
    public Task<Result> UpdateUserAsync(string UserId, UpdateUserRequest request, CancellationToken cancelationToken);
    public Task<Result> ToggleStatusAsync(string UserId, CancellationToken cancelationToken);
    public Task<Result> UnlockUserAsync(string id, CancellationToken cancelationToken);
}
