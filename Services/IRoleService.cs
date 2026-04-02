using BucketSurvey.Api.Contract.Roles;

namespace BucketSurvey.Api.Services;

public interface IRoleService
{
    Task<Result<RoleDetailsResponse>> AddAsync ( RoleRequest request , CancellationToken cancellationToken);
    Task<Result<IEnumerable<RoleResponse>>> GetAllAsync(CancellationToken cancellationToken, bool? hasIncluded);
    Task<Result<RoleDetailsResponse>> GetAsync(string id);
    Task<Result> UpdateAsync(string Id, RoleRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(string Id);

}
