namespace BucketSurvey.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "The specified poll was not found." , StatusCodes.Status404NotFound);

    public static readonly Error CanceledRequest
        = new("Poll.CanceledRequest", "The request is canceld.", StatusCodes.Status400BadRequest);

    public static readonly Error TittleDuplicated
        = new("Poll.TittleDuplicated", "there is poll with the  same tittle" , StatusCodes.Status409Conflict);

}
