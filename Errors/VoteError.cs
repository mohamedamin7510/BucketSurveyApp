namespace BucketSurvey.Api.Errors;

public class VoteError
{

    public static readonly Error VoteDuplicated
        = new Error("Vote.Duplicated", "You voted on this poll before ,Try another poll! ", StatusCodes.Status409Conflict);

    public static readonly Error InvalidQuestions
        = new Error("Vote.Invalidquestions", "Invalid Questions !", StatusCodes.Status400BadRequest);


}
