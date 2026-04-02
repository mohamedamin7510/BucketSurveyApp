namespace BucketSurvey.Api.Contract.User;

public record UserResponse
(
    string Id,
        string FirstName,
            string LastName,
                string Email,
                    bool IsDisabled,
                        IList<string> Roles
);