using BucketSurvey.Api.Contract.Vote;

namespace BucketSurvey.Api.Controllers;

[Route("api/polls/{pollid}/vote")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member)]
public class VotesController(IQuestionService questionService , 
    IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _QuestionService = questionService;
    private readonly IVoteService _VoteService = voteService;


    
    [HttpGet]
    public async Task<IActionResult> GetAvaliable([FromRoute] int pollid, CancellationToken cancellationToken)
    { 

        var result = await _QuestionService.GetAvailiableAsync(pollid, User.GetUserId(), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromRoute] int pollid ,  [FromBody] VoteRequest voteRequest,CancellationToken cancellationToken)
    {
      
        var result = await _VoteService.AddAsync(pollid, User.GetUserId() , voteRequest, cancellationToken);

        return result.IsSuccess ? NoContent() :  result.ToProblem();
    }

}
