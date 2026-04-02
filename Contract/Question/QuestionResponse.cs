using BucketSurvey.Api.Contract.Answer;

namespace BucketSurvey.Api.Contract.Question;

public record QuestionResponse
(
     int Id,
        string Content,
        IEnumerable<AnswerResponse> Answers
);
