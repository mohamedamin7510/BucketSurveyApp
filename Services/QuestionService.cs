using BucketSurvey.Api.Contract.Answer;
using BucketSurvey.Api.Contract.Question;
using Microsoft.EntityFrameworkCore;

namespace BucketSurvey.Api.Services;

public class QuestionService(ApplicationContext context) : IQuestionService
{
    private readonly ApplicationContext _Context = context;



    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollid, CancellationToken cancellationToken)
    {
        var ispollexists = await _Context.polls.AnyAsync(x=>x.Id == pollid);
        if (!ispollexists)
            return Result.Faluire<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var Questions = await _Context.Questions
            .Where(p => p.pollId == pollid)
            .ProjectToType<QuestionResponse>()   
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(Questions); 
    }
    public async Task<Result<QuestionResponse>> GetAsync(int PollId, int Id, CancellationToken cancellationToken)
    {
        
        var quesion = await _Context.Questions
            .Where(x => x.pollId == PollId && x.Id == Id)
            .ProjectToType<QuestionResponse>()
            .SingleOrDefaultAsync(cancellationToken);

        if (quesion is null)
            return Result.Faluire<QuestionResponse>(QuestionError.QuesitonNofound);


        return Result.Success(quesion);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest Request, CancellationToken cancellationToken = default)
    {


        var IsPollExists = await _Context.polls.AnyAsync(x=>x.Id == PollId , cancellationToken);
        if (!IsPollExists)
            return Result.Faluire<QuestionResponse>(PollErrors.PollNotFound);


        var IsDuplicatedQuestion = await _Context.Questions.AnyAsync(q=>q.Content == Request.Content && q.pollId == PollId , cancellationToken);
        if (IsDuplicatedQuestion)
             return Result.Faluire<QuestionResponse>(QuestionError.QuesitonContentDuplicated);


        var question = Request.Adapt<Question>();  
        
        question.pollId = PollId; 
        
        await _Context.Questions.AddAsync(question ,cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }
    public async Task<Result> UpdateAsync(int pollid, int id, QuestionRequest Request , CancellationToken cancellationToken)
    {

        // if the quesiton duplicted or not 
        var IsDuplicated = await _Context.Questions.AnyAsync
            (
                x=> x.pollId == pollid
                && x.Content == Request.Content
                && x.Id != id 
            );
        if (IsDuplicated)
            Result.Faluire(QuestionError.QuesitonContentDuplicated);

        // if the equesiton  exists or not 
        var question = await _Context.Questions
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.pollId == pollid && x.Id == id ,cancellationToken);
        if (question is null)
            Result.Faluire(QuestionError.QuesitonNofound);

        question!.Content = Request.Content;

        var currentanswers = question!.Answers.Select(x=> x.Content).ToList();

        var newanswers = Request.Answers.Except(currentanswers).ToList();

        newanswers.ForEach(Answercontent=>
            {
                question.Answers.Add(new Answer() { Content = Answercontent} );
            }
        );
        


        foreach (var item in question.Answers)
        {
            item.IsActive = Request.Answers.Contains(item.Content);
        }

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(); 
    }
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailiableAsync(int pollid, string? userId, CancellationToken cancellationToken)
    {
        var IsExistsvalidvotingpoll= await _Context.polls.
            Where(x=>x.Id ==pollid)
            .AnyAsync(x => x.IsPublished
            && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow) ,cancellationToken);

        if (!IsExistsvalidvotingpoll)
            return Result.Faluire<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var IsVoted = await  _Context.Votes.AnyAsync(x=> x.pollId == pollid && x.userId == userId ,cancellationToken);

        if (IsVoted)
            return Result.Faluire<IEnumerable<QuestionResponse>>(VoteError.VoteDuplicated);

        var questions = _Context.Questions.Include(x => x.Answers)
            .Where(x => x.IsActive && pollid == x.pollId)
            .Select(x => new QuestionResponse(x.Id, x.Content,
            x.Answers
            .Where(x => x.IsActive)
            .Select(answ => new AnswerResponse(answ.Id, answ.Content))));

        return Result.Success<IEnumerable<QuestionResponse>>(questions); 
    }
    public async Task<Result> ToggleStatusAsync(int PollId, int Id, CancellationToken cancellationToken)
    {
         var quesion = await _Context.Questions
         .Where(x => x.pollId == PollId && x.Id == Id)   
         .SingleOrDefaultAsync(cancellationToken);

        if (quesion is null)
            return Result.Faluire<QuestionResponse>(QuestionError.QuesitonNofound);

        quesion.IsActive = !quesion.IsActive;

        await _Context.SaveChangesAsync(cancellationToken);

        return  Result.Success();
    }

}
