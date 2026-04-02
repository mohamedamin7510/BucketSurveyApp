namespace BucketSurvey.Api.Controllers;


[ApiController()]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Member" )]
public class PollsController(IPollService PollService) : ControllerBase
{
    private readonly IPollService _PollService = PollService;

    [HttpGet("")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken =default!)
    {
        var Response = await _PollService.GetAllAsync(cancellationToken);
        return  Ok(Response.Value);
    }


    [Authorize(Roles = DefaultRoles.Member)]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken =default!)
    {
        var Response = await _PollService.GetCurrentAsync(cancellationToken);

        return  Ok(Response.Value);
    }

    [HasPermission(Permissions.GetPolls)]
    [HttpGet("{ID}")]
    public async Task<IActionResult> GetPoll([FromRoute]int ID, CancellationToken cancellationToken)
    {
        var pollresult = await _PollService.GetAsync(ID, cancellationToken)!;

        return pollresult.IsSuccess ? Ok(pollresult.Value) : pollresult.ToProblem();
    }


    [HttpPost("")]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> AddAsync([FromBody] PollRequest PollRequest, CancellationToken cancellationToken = default)
    {
        var AddedResourceResult = _PollService.AddAsync(PollRequest, cancellationToken).Result;

        return AddedResourceResult.IsSuccess ?
         CreatedAtAction(nameof(GetPoll), new { ID = AddedResourceResult.Value.Id },AddedResourceResult.Value):AddedResourceResult.ToProblem();     
    }


    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request  , CancellationToken cancellationToken = default)
    {
        var result = await _PollService.UpdateAsync(Id, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
   

    [HttpDelete("{Id}")]
    [HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete( [FromRoute] int Id  , CancellationToken cancellationToken = default)
    {
        var IsDeletedResult = await _PollService.DeleteAsync(Id, cancellationToken);

        return IsDeletedResult.IsSuccess ? NoContent() : IsDeletedResult.ToProblem();
    }



    [HttpPut("{Id}/togglestatus")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> ToggleIsPublishedStatus  ([FromRoute] int Id, CancellationToken cancellationToken = default)
    {
        var IsUpdatedRes = await _PollService.ToggleIsPublishedStatus(Id, cancellationToken);

        return IsUpdatedRes.IsSuccess ? NoContent() : IsUpdatedRes.ToProblem();
    }


}
