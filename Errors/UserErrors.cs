namespace BucketSurvey.Api.Errors;

public class UserErrorMessages
{
    public static readonly Error Invalidcredentails =
        new(Code: "User.Invalidcredentails", Descreption: "Sorry can't register because Email or Password is wrong Or both", StatusCodes.Status401Unauthorized);


    public static Error InvalidToken = new("Token.InvalidToken", "This token is  not valid for the use", StatusCodes.Status401Unauthorized);


}
