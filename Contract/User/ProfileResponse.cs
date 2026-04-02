namespace BucketSurvey.Api.Contract.User;

public record ProfileResponse
(
  string FirstName,
  string lastName ,
  string email,
  string UserName
);
