namespace BucketSurvey.Api.Controllers;

[Route("[controller]")]
[ApiController]


public class AuthenticationController(IAuthService authService,
    ILogger<AuthenticationController> logger,
    IOptions<JwtOptions> Jwtoptions
    ) : ControllerBase
{
    private readonly IAuthService _AuthService = authService;
    private readonly JwtOptions _Jwtoptions = Jwtoptions.Value;
    private readonly ILogger<AuthenticationController> _Logger = logger;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] LoginRequest AuthRequest, CancellationToken cancellationToken = default)
    {
        _Logger.LogInformation("Logging From Auth: email is : {email} and the password is : {password}", AuthRequest.Email, AuthRequest.Password);

        var ResultResponse = await _AuthService.GetTokenAsync(AuthRequest.Email, AuthRequest.Password, cancellationToken);

        return ResultResponse.IsSuccess ? Ok(ResultResponse.Value) : ResultResponse.ToProblem();
    }



    [HttpPost("refresh")]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest reftokenRequest, CancellationToken cancellationToken = default)
    {
        var ResultResponse = await _AuthService.GetRefreshTokenAsync(reftokenRequest.token, reftokenRequest.refreshToken, cancellationToken);

        return ResultResponse.IsSuccess ? Ok(ResultResponse.Value) : ResultResponse.ToProblem();

    }


    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest reftokenRequest, CancellationToken cancellationToken = default)
    {
        var IsRevoked = await _AuthService.RevokeRefreshTokenAsync(reftokenRequest.token, reftokenRequest.refreshToken, cancellationToken);

        return IsRevoked.Match(
            authResponse => Ok("Revoked Successefully!"),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error._Code, detail: error._Descreption
            ));

    }



    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.RegisterAsync(request, cancellationToken);

        return resutl.IsSuccess ? Ok()
            : resutl.ToProblem();
    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ConfirmEmailAsync(request, cancellationToken);

        return resutl.IsSuccess ? Ok()
            : resutl.ToProblem();
    }


    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ResendConfirmEmailAsync(request, cancellationToken);

        return resutl.IsSuccess ? Ok()
            : resutl.ToProblem();
    }


    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.SendResetPassCodeAsync(request, cancellationToken);

        return resutl.IsSuccess ? Created() : resutl.ToProblem();
    }


    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ResetPasswordAsync(request, cancellationToken);

        return resutl.IsSuccess ? NoContent() : resutl.ToProblem();
    }



    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _AuthService.GoogleLoginAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
