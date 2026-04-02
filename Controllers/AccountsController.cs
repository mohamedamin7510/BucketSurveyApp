using BucketSurvey.Api.Contract.User;

namespace BucketSurvey.Api.Controllers;


[Route("account")]
[ApiController]
[Authorize]
public class AccountsController(IUserService userService , IConfiguration configuration , ILogger<AccountsController> logger) : ControllerBase
{
    private readonly IUserService _UserService = userService;
    private readonly IConfiguration _Configuration = configuration;
    private readonly ILogger<AccountsController> _Logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetUser()
    {
       var response = await _UserService.GetUserAsync(User.GetUserId()!);

       _Logger.LogError(_Configuration["myname"]);

       return Ok(response.Value); 
    }

    [HttpPut("")] 
    public async Task<IActionResult> UpdateUser([FromBody] ProfileRequest request)
    {
       await _UserService.UpdateUserAsync(User.GetUserId()!, request);

       return NoContent(); 
    }

    [AllowAnonymous]
    [HttpPut("change-password")] 
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
      var result =  await _UserService.ChangePasswordAsync(User.GetUserId()!, request);

       return result.IsSuccess ?  NoContent()  : result.ToProblem(); 
    }





}
