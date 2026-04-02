namespace BucketSurvey.Api.Errors;

public class UserError
{
    public static readonly Error Invalidcredentails = new("User.Invalidcredentails", "Sorry can't register because email or password is wrong Or both",      
          StatusCodes.Status401Unauthorized);

    public static Error InvalidToken = new("User.InvalidToken",
            "This token is  not valid for the use", StatusCodes.Status401Unauthorized);

    public static Error UserLockedOut = new("User.LockedOut",
            "You are blocking for a short time beacuse you entered the password 4 times wrong", StatusCodes.Status401Unauthorized);

    public static Error UserNotFound = new Error("User.NotFound",
    "This user is not founded in the system !", StatusCodes.Status404NotFound);

    public static Error DuplicatedEmail = new Error("User.DuplicatedEmail",
        "This email is already registered", StatusCodes.Status409Conflict);

    public static Error NotConfirmedEmail = new Error("User.NotConfirmedEmail",
        "This email is not confirmed", StatusCodes.Status401Unauthorized);

    public static Error InvalidCode = new Error("User.InvalidCode",
        "This code is not in correct format!", StatusCodes.Status400BadRequest);

    public static Error ActiveConfirmedEmail = new Error("User.ActiveConfirmedEmail",
        "This email is actually confirmed!", StatusCodes.Status400BadRequest);

}
