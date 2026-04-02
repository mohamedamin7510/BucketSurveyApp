namespace BucketSurvey.Api.Contract.Vote;

public record VoteAnswerRequest
(
    int QuestionId , 
    int AnswerId 
);

