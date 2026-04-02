namespace BucketSurvey.Api.Contract.Authentication;

public record RefreshTokenRequest(
    string token , 
    string refreshToken
);

