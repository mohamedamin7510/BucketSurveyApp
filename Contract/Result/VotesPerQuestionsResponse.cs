namespace BucketSurvey.Api.Contract.Result;

public record VotesPerQuestionsResponse
(

    string Question , 
    IEnumerable<CountAnswerVoteResponse>  NumberofAnswerVotes 
);
