namespace BucketSurvey.Api.Contract.Result;

public record VotePerDayResponse
(
  DateOnly Date , 
      int VoteNumbers
);