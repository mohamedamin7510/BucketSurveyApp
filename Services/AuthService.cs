using Google.Apis.Auth;

namespace BucketSurvey.Api.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    ApplicationContext context,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IJwtProvider jwtProvider,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration
    ) : IAuthService
{

    private UserManager<ApplicationUser> _UserManager { get; } = userManager;
    private readonly ApplicationContext _Context = context;
    private readonly SignInManager<ApplicationUser> _SignInManager = signInManager;
    private readonly IJwtProvider _JwtProvider = jwtProvider;
    private readonly IEmailSender _EmailSender = emailSender;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;
    private readonly IConfiguration _Configuration = configuration;
    private readonly int _RefreshTokenExpiryDate = 14;
    private readonly ILogger<AuthService> _Logger = logger;



    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user =  await _UserManager.FindByEmailAsync(email);

        if (user is null)
            return  Result.Faluire<AuthResponse>(UserError.Invalidcredentails);



       var res  = await _SignInManager.PasswordSignInAsync(user , password ,false , true);

        if (res.Succeeded)
        {
            (var UserRoles ,var Permissions) = await GetUserRolesandTheirPermissions(user, cancellationToken)!;

            var TokenInfo = _JwtProvider.GenerateToken(user, UserRoles, Permissions!);

            var RefreshToken = RefreshTokenGenerated();

            var ExpiryRefreshTokenDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

            user.RefreshTokens.Add(new RefreshToken() { refToken = RefreshToken, ExpiresAt = ExpiryRefreshTokenDate });

            await _UserManager.UpdateAsync(user);

            var resultResponse = new AuthResponse(user.Id,
                Email: user.Email!,
                FirstName: user.FirstName!,
                LastName: user.LastName!,
                Token: TokenInfo.Token,
                ExpiresIn: TokenInfo.Expiresin * 60,
                RefreshToken: RefreshToken,
                ExpiresAt: ExpiryRefreshTokenDate);


            return Result.Success(resultResponse);
        }

        var error = res.IsLockedOut ? UserError.UserLockedOut
            : res.IsNotAllowed ? UserError.NotConfirmedEmail
            : UserError.Invalidcredentails;

        return  Result.Faluire<AuthResponse>(error);
    }


    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userid = _JwtProvider.ValidateToken(token);
        if (userid is null)
            return Result.Faluire<AuthResponse>(UserError.InvalidToken);



        var user = await _UserManager.FindByIdAsync(userid);
        if (user is null)
            return Result.Faluire<AuthResponse>(UserError.InvalidToken);



        var refreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.refToken == refreshtoken && rt.IsActive);

        if (refreshToken is null)
            return Result.Faluire<AuthResponse>(UserError.InvalidToken);

        if (user.LockoutEnd > DateTime.UtcNow)
        {
            return Result.Faluire<AuthResponse>(UserError.UserLockedOut);
        }

        refreshToken.RevokedAt = DateTime.UtcNow;

        var result = await  GetUserRolesandTheirPermissions(user , cancellationToken);

        var (newtoken, Expiresin) = _JwtProvider.GenerateToken(user , result.UserRoles , result.RolePermissions);

        var newrefreshtoken = RefreshTokenGenerated();

        var ExpiryRefreshTokenDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(
            new RefreshToken() { refToken = newrefreshtoken, ExpiresAt = ExpiryRefreshTokenDate }
            );
        await _UserManager.UpdateAsync(user);

        var response = Result.Success(
           new AuthResponse(user.Id, user.Email!, user.LastName!, user.FirstName!, Token: newtoken, ExpiresIn: Expiresin * 60,
            RefreshToken: newrefreshtoken,
            ExpiresAt: ExpiryRefreshTokenDate
            ));


        return response;


    }

    public async Task<OneOf<bool,Error>> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userid = _JwtProvider.ValidateToken(token);
        if (userid is null)
            return UserError.InvalidToken;  

        // give me this user with this id if will be exist in the database
        var user = await _UserManager.FindByIdAsync(userid);
        if (user is null)
            return UserError.InvalidToken;

        // validate the refresh token 
        var refreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.refToken == refreshtoken && rt.IsActive);
        if (refreshToken is null)
            return UserError.InvalidToken;

        refreshToken.RevokedAt = DateTime.UtcNow;

        await _UserManager.UpdateAsync(user);


        return true; 


    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var IsEmailExisted = await _Context.Users.AnyAsync(x => x.Email == request.email, cancellationToken);

        if (IsEmailExisted)
            return Result.Faluire(UserError.DuplicatedEmail);

        ApplicationUser newuser = new()
        {
            Email = request.email,
            FirstName = request.firstName,
            LastName = request.lastName,
            UserName = request.email,
        };

        var result = await _UserManager.CreateAsync(newuser, request.password);

        if (result.Succeeded)
        {
            var Code = await _UserManager.GenerateEmailConfirmationTokenAsync(newuser);
            Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));

            _Logger.LogInformation("Confirmation Code:{Code}", Code);

            await _UserManager.AddToRoleAsync(newuser, DefaultRoles.Member);
            await SendEmailConfirmation(newuser, Code);

            return Result.Success();
        }

        var error = result.Errors.FirstOrDefault();
        return Result.Faluire(new Error(
            error?.Code ?? "Register.Failed",
            error?.Description ?? "The registration is not completed.",
            StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByIdAsync(request.userId) is not { } User)
            return Result.Faluire(UserError.InvalidCode);

        if (User.EmailConfirmed)
            return Result.Faluire(UserError.NotConfirmedEmail);

        string Code = request.Code;

        try
        {
             Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        }
        catch(FormatException)
        {
            return Result.Faluire(UserError.InvalidCode);
        }

        var finalresult = await _UserManager.ConfirmEmailAsync(User, Code);

        if (finalresult.Succeeded)
        {
            return Result.Success();
        }
        var error = finalresult.Errors.FirstOrDefault()!;

        return Result.Faluire(new Error (error.Code,error.Description,StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        if (await _UserManager.FindByEmailAsync(request.email) is not { } User)
            return Result.Success();

        if (User.EmailConfirmed)
            return Result.Faluire(UserError.ActiveConfirmedEmail);

        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _UserManager.GenerateEmailConfirmationTokenAsync(User)));

        _Logger.LogInformation("Confirmation Code: {code}",code);

        await SendEmailConfirmation(User, code);

        return  Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByEmailAsync(request.email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Faluire(UserError.InvalidCode);

        IdentityResult result = null!; 
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));

            result =  await _UserManager.ResetPasswordAsync(user, code, request.newpassword);
        }
        catch(FormatException)
        {
            result = IdentityResult.Failed(_UserManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.FirstOrDefault();

        return Result.Faluire(new Error(error!.Code , error.Description , StatusCodes.Status401Unauthorized));
    }

    public async Task<Result> SendResetPassCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByEmailAsync(request.email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Faluire(UserError.NotConfirmedEmail);

        var code = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(await _UserManager.GeneratePasswordResetTokenAsync(user)));

        _Logger.LogInformation(code);

        var origin = _HttpContextAccessor.HttpContext?.Request.Headers.Origin.ToString();

        var HtmlBody = EmailBodyBuilder.GenerateEmailBody("ResetPassword", new Dictionary<string, string>()
        {
            {"{{CODE}}", code},
            {"{{FullName}}", user.FirstName + " " + user.LastName},
            {"{{URI}}", $"{origin}/resetpass?email={user.Email}&&code={code}"}
        });

        await _EmailSender.SendEmailAsync(user.Email!, "BucketSurvey: Reset Password", HtmlBody);

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {

        var AllowedClientIds = _Configuration.GetSection("ExternalLogins:Google:AllowedClientIds").Get<string[]>();
           
        GoogleJsonWebSignature.Payload payload = null!;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync( request.IdToken,
               
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = AllowedClientIds
                }
                );
        }
        catch
        {
            return Result.Faluire<AuthResponse>(UserError.Invalidcredentails);
        }


        if (string.IsNullOrWhiteSpace(payload.Email) || !payload.EmailVerified)
        {
            return Result.Faluire<AuthResponse>(UserError.Invalidcredentails);
        }


        var user = await _UserManager.FindByLoginAsync(Providers.Googlerovider, payload.Subject);

        if (user is null)
        {
            user = await _UserManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName ?? "Google",
                    LastName = payload.FamilyName ?? "User",
                    EmailConfirmed = true
                };

                var createResult = await _UserManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    var createError = createResult.Errors.First();
                    return Result.Faluire<AuthResponse>(
                        new Error(createError.Code, createError.Description, StatusCodes.Status400BadRequest));
                }

                await _UserManager.AddToRoleAsync(user, DefaultRoles.Member);
            }

            var addLoginResult = await _UserManager.AddLoginAsync(user, new UserLoginInfo(Providers.Googlerovider, providerKey, Providers.Googlerovider));

            if (!addLoginResult.Succeeded)
            {
                var loginError = addLoginResult.Errors.First();

                return Result.Faluire<AuthResponse>(new Error(loginError.Code, loginError.Description, StatusCodes.Status400BadRequest));
            }
        }


        if (user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return Result.Faluire<AuthResponse>(UserError.UserLockedOut);
        }

        var response = await LoginWithGoogleExternalApiAsync(user, cancellationToken);

        return Result.Success(response);
    }


    private string RefreshTokenGenerated()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendEmailConfirmation(ApplicationUser User, string code)
    {
        var origin = _HttpContextAccessor.HttpContext?.Request.Headers.Origin;

        var BuilderMessage = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",

        new Dictionary<string, string>
            {
                        {"{{name}}", User.FirstName + " "+ User.LastName },
                        { "{{action_url}}", $"{origin}/Authentication/confirm-email?userid={User.Id}&&code={code}" }
            }
        );


        BackgroundJob.Enqueue(
            () => _EmailSender.SendEmailAsync(User.Email!, "️✅ Survey Basket: Email Confirmation", BuilderMessage));
    }
    private async Task<(IEnumerable<string> UserRoles,IEnumerable<string> RolePermissions)>GetUserRolesandTheirPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var UserRoles = await _UserManager.GetRolesAsync(user);

        var Permissions = await
          (
              from R in _Context.Roles
              join RC in _Context.RoleClaims
              on R.Id equals RC.RoleId
              where UserRoles.Contains(R.Name!)
              select RC.ClaimValue

            )
        .Distinct()
        .ToListAsync(cancellationToken);

        return (UserRoles, Permissions);
    }

    private async  Task<AuthResponse> LoginWithGoogleExternalApiAsync(ApplicationUser user , CancellationToken cancellationToken)
    {

        (var userRoles, var permissions) = await GetUserRolesandTheirPermissions(user, cancellationToken);

        var tokenInfo = _JwtProvider.GenerateToken(user, userRoles, permissions!);

        var refreshToken = RefreshTokenGenerated();

        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(new RefreshToken
        {
            refToken = refreshToken,
            ExpiresAt = refreshTokenExpiry
        });

        await _UserManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email!,
            user.FirstName!,
            user.LastName!,
            tokenInfo.Token,
            tokenInfo.Expiresin * 60,
            refreshToken,
            refreshTokenExpiry);

        return response;
    }

}
