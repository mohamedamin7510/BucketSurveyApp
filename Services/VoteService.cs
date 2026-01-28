using BucketSurvey.Api.Contract.Vote;

namespace BucketSurvey.Api.Services;

public class VoteService(ApplicationContext applicationContext) : IVoteService
{
    private readonly ApplicationContext _Context = applicationContext;

    public async Task<Result> AddAsync(int pollid, string? userid, VoteRequest Request, CancellationToken cancellationToken = default)
    {

        var IsExists = await _Context.polls
                   .AnyAsync(X => X.Id == pollid
                    && X.IsPublished
                    && X.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                    && X.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!IsExists)
            return Result.Faluire(PollErrors.PollNotFound);

        var IsVoted = await _Context.Votes.AnyAsync(x => x.pollId == pollid && x.userId == userid, cancellationToken);

        if (IsVoted)
            return Result.Faluire(VoteError.VoteDuplicated);

        var AvaliableQuesionIds = await _Context.Questions
            .Where(x => x.pollId == pollid && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (!AvaliableQuesionIds.SequenceEqual(Request.Answers.Select(x => x.QuestionId)))
        {
            return Result.Faluire(VoteError.InvalidQuestions);
        }

        var vote = new Vote
        {
            pollId = pollid,
            userId = userid,
            VoteAnswers = Request.Answers.Adapt<IEnumerable<VoteAnswers>>().ToList()
        };


        await _Context.Votes.AddAsync(vote, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }



}