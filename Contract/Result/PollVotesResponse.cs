namespace BucketSurvey.Api.Contract.Result;

public record PollVotesResponse
(
    string Tittle ,
    IEnumerable<VoteResponse> Votes 
);
