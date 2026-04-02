namespace BucketSurvey.Api.Contract.Roles;

public record RoleRequest
(
    string Name,
        IEnumerable<string> Permissions 
);