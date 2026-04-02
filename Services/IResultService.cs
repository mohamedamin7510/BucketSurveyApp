
using BucketSurvey.Api.Contract.Result;

namespace BucketSurvey.Api.Services;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesResultsAsync(int pollid, CancellationToken cancelationToken);
    Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollid, CancellationToken cancelationToken);
    Task<Result<IEnumerable<VotesPerQuestionsResponse>>> GetVotesPerQuestionAsync(int pollid, CancellationToken cancelationToken);
}
