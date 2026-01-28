namespace BucketSurvey.Api.Errors;

public class QuestionError
{
    public static readonly Error QuesitonNofound
        = new Error("Question.Nofound", "The question not exists in the database", StatusCodes.Status404NotFound);
  
    public static readonly Error QuesitonContentDuplicated 
        = new Error("Question.DuplicatedContent",
            "The question content is not allowed to repeat within same poll", StatusCodes.Status409Conflict);


}
