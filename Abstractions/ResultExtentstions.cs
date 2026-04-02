namespace BucketSurvey.Api.Abstractions;

public static class ResultExtentstions
{
    public static ObjectResult ToProblem(this Result result)
    {
        var problem = Results.Problem(statusCode: result.error._StatusCode);
        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
        problemDetails!.Extensions =  
            new Dictionary<string, object?>
            {
                {
                    "errors", new { 
                        result.error._Code , 
                        result.error._Descreption                     
                    }
                }
            };

        return new ObjectResult(problemDetails);
    }

}

