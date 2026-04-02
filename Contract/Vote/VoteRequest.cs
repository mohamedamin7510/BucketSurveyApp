namespace BucketSurvey.Api.Contract.Vote;

public record VoteRequest
(
    IEnumerable<VoteAnswerRequest> Answers 
);