using BucketSurvey.Api.Contract.Vote;

namespace BucketSurvey.Api.Services;

public interface IVoteService
{
    Task<Result> AddAsync(int pollid, string? userid, VoteRequest Request, CancellationToken cancellationToken = default);
}
