using BucketSurvey.Api.Contract.Question;

namespace BucketSurvey.Api.Services;

public interface IQuestionService
{
     Task<Result<QuestionResponse>> AddAsync(int PollId , QuestionRequest Request , CancellationToken cancellationToken = default!);

     Task<Result> UpdateAsync(int pollid, int id, QuestionRequest Request ,  CancellationToken cancellationToken);

     Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken);

     Task<Result<QuestionResponse>> GetAsync(int PollId, int Id , CancellationToken cancellationToken);

     Task<Result<IEnumerable<QuestionResponse>>> GetAvailiableAsync(int pollid, string? userId, CancellationToken cancellationToken);
     Task<Result> ToggleStatusAsync(int PollId, int Id, CancellationToken cancellationToken);


}