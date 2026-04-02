namespace BucketSurvey.Api.Contract.Authentication;

public record RegisterRequest
(
    string email,
        string password,
            string firstName,
                string lastName,
                   string phonenumber
);
