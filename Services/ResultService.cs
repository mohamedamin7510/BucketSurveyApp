using BucketSurvey.Api.Contract.Result;

namespace BucketSurvey.Api.Services;

public class ResultService(ApplicationContext context) : IResultService
{
    private readonly ApplicationContext _Context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesResultsAsync(int pollid, CancellationToken cancelationToken)
    {
        var pollvoteResults = await _Context.polls
            .Where(x => x.Id == pollid)
            .Select(x => new PollVotesResponse(
                x.Title,
           x.Votes.Select(x => new VoteResponse(

                $"{x.User.FirstName} {x.User.LastName}",
                x.SubmitAt,
                x.VoteAnswers.Select(x => new QuestionAnswerResponse(x.question.Content, x.Answer.Content))
                ))
                ))
            .SingleOrDefaultAsync(cancelationToken);


        return pollvoteResults is null ?
            Result.Faluire<PollVotesResponse>(PollErrors.PollNotFound)
            : Result.Success<PollVotesResponse>(pollvoteResults);
    }

    public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollid, CancellationToken cancelationToken)
    {

        bool IsPollExists = await _Context.polls.AnyAsync(x => x.Id == pollid, cancelationToken);
        if (!IsPollExists)
            return Result.Faluire<IEnumerable<VotePerDayResponse>>(PollErrors.PollNotFound);

        var VotesPerDay = await _Context.Votes
            .Where(x => x.pollId == pollid)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmitAt) })
            .Select(x => new VotePerDayResponse(x.Key.Date, x.Count()))
            .ToListAsync(cancelationToken);

        return Result.Success<IEnumerable<VotePerDayResponse>>(VotesPerDay);
    }


    public async Task<Result<IEnumerable<VotesPerQuestionsResponse>>> GetVotesPerQuestionAsync(int pollid, CancellationToken cancelationToken)
    {
        bool IsPollExists = await _Context.polls.AnyAsync(x => x.Id == pollid, cancelationToken);
        if (!IsPollExists)
            return Result.Faluire<IEnumerable<VotesPerQuestionsResponse>>(PollErrors.PollNotFound);


        var Response = await _Context.
            polls.Where(x => x.Id == pollid)
            .Select(x => x.Questions
                .Select(x => new VotesPerQuestionsResponse(x.Content,
                 x.VoteAnswers.GroupBy(x => x.AnswerId)
                 .Select(x => new CountAnswerVoteResponse(x.Select(x=>x.Answer.Content).FirstOrDefault()!, x.Count()))

            )))
            .SingleOrDefaultAsync(cancelationToken);

        
        return Result.Success(Response!);

    }

 
}
