using BucketSurvey.Api.Contract.User;

namespace BucketSurvey.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _UserService = userService;

    [HttpGet]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll(CancellationToken cancelationToken)
    {
        var response = await _UserService.GetAllUsersAsync(cancelationToken);

        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }



    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> Get([FromRoute] string Id, CancellationToken cancelationToken)
    {
        var response = await _UserService.GetUserAsync(Id, cancelationToken);

        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }


    [HttpPost]
    [HasPermissionAttribute(Permissions.AddUsers)]
    public async Task<IActionResult> Add([FromBody] UserRequest request ,  CancellationToken cancelationToken)
    {
        var response = await _UserService.AddUserAsync(request , cancelationToken);

        return response.IsSuccess ?  CreatedAtAction(nameof(Get) , new { Id = response.Value.Id}, response.Value)
            : response.ToProblem();
    }


    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> Update([FromRoute]string Id, [FromBody] UpdateUserRequest request, CancellationToken cancelationToken)
    {
        var response = await _UserService.UpdateUserAsync( Id , request , cancelationToken);

        return response.IsSuccess ? NoContent() : response.ToProblem();
    }


    [HttpPut("{Id}/toggle-status")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string Id , CancellationToken cancelationToken)
    {

        var res = await  _UserService.ToggleStatusAsync(Id,cancelationToken);

        return res.IsSuccess ? NoContent() : res.ToProblem();
    }


    [HttpPut("{Id}/unlock")]
    [HasPermission(Permissions.UpdateUsers)]

    public async Task<IActionResult> Unlock([FromRoute] string Id, CancellationToken cancelationToken)
    {
        var res = await _UserService.UnlockUserAsync(Id, cancelationToken);

        return res.IsSuccess ? NoContent() : res.ToProblem();
    }


}
