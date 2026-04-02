namespace BucketSurvey.Api.Controllers;



[Route("api/polls/{pollid}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _QuestionService = questionService;


    [HttpGet("")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> GetAll([FromRoute] int pollid , CancellationToken cancellationToken)
    {
        var result = await _QuestionService.GetAllAsync(pollid, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(); 
    }


    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> Get([FromRoute] int pollid, [FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await  _QuestionService.GetAsync(pollid, Id , cancellationToken);

        return result.IsSuccess ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpPost("")]
    [HasPermission(Permissions.AddQuestions)]
    public async Task<IActionResult> Add([FromRoute] int pollid, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _QuestionService.AddAsync(pollid, request, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(actionName: nameof(Get), routeValues: new { pollid, Id = result.Value.Id } , value: result.Value);

        return result.ToProblem();
    }


    [HttpPut("{Id}/togglestatus")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollid, [FromRoute] int Id, CancellationToken cancellationToken = default)
    {
        var IsUpdatedRes = await _QuestionService.ToggleStatusAsync(pollid, Id, cancellationToken);

        return IsUpdatedRes.IsSuccess ? NoContent() : IsUpdatedRes.ToProblem();     
    }

    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> Update([FromRoute] int pollid, [FromRoute] int Id, QuestionRequest questionRequest , CancellationToken cancellationToken = default)
    {

        var result = await _QuestionService.UpdateAsync(pollid, Id, questionRequest, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }




}