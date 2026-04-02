namespace BucketSurvey.Api.Contract.Poll;

public record PollRequest(
     string Title,
      string Summary,
       DateOnly StartsAt,
          DateOnly EndsAt

    );
