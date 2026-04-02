namespace BucketSurvey.Api.Contract.Result;

public record VoteResponse
(
    string VoterName , 
    DateTime VotedTime , 
    IEnumerable<QuestionAnswerResponse> QuestionAnswers
);
