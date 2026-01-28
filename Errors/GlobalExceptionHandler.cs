using Microsoft.AspNetCore.Diagnostics;

namespace BucketSurvey.Api.Errors;

public class GlobalExceptionHandler (ILogger<GlobalExceptionHandler> logger): IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _Logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _Logger.LogError("Some thing wrong happened : {message}", exception.Message);
        var problem = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError, 
            Title = "Internal server error ",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError; 
        await httpContext.Response.WriteAsJsonAsync(problem);
        return true;
    }

}
