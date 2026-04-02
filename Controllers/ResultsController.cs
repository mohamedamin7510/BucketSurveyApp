using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BucketSurvey.Api.Controllers;

[Route("api/polls/{pollid}/[controller]")]
[ApiController]
[HasPermission(Permissions.GetResults)]

public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _ResultService = resultService;

    [HttpGet("raw-data")]
    public async Task<IActionResult> GetPollVotesResults([FromRoute] int pollid, CancellationToken cancelationToken)
    {
        var result = await _ResultService.GetPollVotesResultsAsync(pollid, cancelationToken);

        return result.IsSuccess ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpGet("vote-per-day")]
    public async Task<IActionResult> GetVotePerDay([FromRoute] int pollid, CancellationToken cancelationToken)
    {
        var result = await _ResultService.GetVotesPerDayAsync(pollid, cancelationToken);

        return result.IsSuccess ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpGet("votes-per-question")]
    public async Task<IActionResult> GetVotePerQuestion([FromRoute] int pollid, CancellationToken cancelationToken)
    {
        var result = await _ResultService.GetVotesPerQuestionAsync(pollid, cancelationToken);

        return result.IsSuccess ? Ok(result.Value)
            : result.ToProblem();
    }


}



