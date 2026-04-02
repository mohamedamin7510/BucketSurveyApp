namespace BucketSurvey.Api.Contract.User;

public record UserRequest
(
     string FirstName , 
     string LastName,
     string Email , 
     string Password , 
     IList<string> Roles 
);
