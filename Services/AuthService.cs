using OneOf;
using System.Security.Cryptography;

namespace BucketSurvey.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager , IJwtProvider jwtProvider) : IAuthService
{
    private readonly IJwtProvider _JwtProvider = jwtProvider;
    private  UserManager<ApplicationUser> _UserManager { get; } = userManager;
    private readonly int _RefreshTokenExpiryDate = 14; 


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user =  await _UserManager.FindByEmailAsync(email);
        if (user is null)
            return  Result.Faluire<AuthResponse>(UserErrorMessages.Invalidcredentails);

        bool IsPasswordValid =   await _UserManager.CheckPasswordAsync(user, password);
        if (!IsPasswordValid)
            return Result.Faluire<AuthResponse>(UserErrorMessages.Invalidcredentails);


        var TokenInfo = _JwtProvider.GenerateToken(user);

        var RefreshToken = RefreshTokenGenerated();
        var ExpiryRefreshTokenDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(
            new RefreshToken() { refToken = RefreshToken , ExpiresAt = ExpiryRefreshTokenDate}
            );
         await _UserManager.UpdateAsync(user);

        var resultResponse = new AuthResponse(user.Id,
            user.Email!,
            user.LastName!,
            user.FirstName!,
            Token: TokenInfo.Token,
            ExpiresIn: TokenInfo.Expiresin * 60,
            RefreshToken: RefreshToken,
            ExpiresAt: ExpiryRefreshTokenDate);

        return Result.Success(resultResponse);

    }



    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userid = _JwtProvider.ValidateToken(token);
        if (userid is null)
          return  Result.Faluire<AuthResponse>(UserErrorMessages.InvalidToken);

        var user = await _UserManager.FindByIdAsync(userid);
        if (user is null)
            return Result.Faluire<AuthResponse>(UserErrorMessages.InvalidToken);

 
        var refreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.refToken == refreshtoken && rt.IsActive);
        if (refreshToken is null)
            return Result.Faluire<AuthResponse>(UserErrorMessages.InvalidToken);

        refreshToken.RevokedAt = DateTime.UtcNow;


        var (newtoken, Expiresin) = _JwtProvider.GenerateToken(user);
        var newrefreshtoken = RefreshTokenGenerated();

        var ExpiryRefreshTokenDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(
            new RefreshToken() { refToken = newrefreshtoken, ExpiresAt = ExpiryRefreshTokenDate }
            );
        await _UserManager.UpdateAsync(user);

        var response = Result.Success(  
            new AuthResponse(user.Id,user.Email!,user.LastName!,user.FirstName!,Token: newtoken, ExpiresIn: Expiresin * 60,
            RefreshToken: newrefreshtoken,
            ExpiresAt: ExpiryRefreshTokenDate
            ));


        return response;


    }


    public async Task<OneOf<bool,Error>> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userid = _JwtProvider.ValidateToken(token);
        if (userid is null)
            return UserErrorMessages.InvalidToken;  

        // give me this user with this id if will be exist in the database
        var user = await _UserManager.FindByIdAsync(userid);
        if (user is null)
            return UserErrorMessages.InvalidToken;

        // validate the refresh token 
        var refreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.refToken == refreshtoken && rt.IsActive);
        if (refreshToken is null)
            return UserErrorMessages.InvalidToken;

        refreshToken.RevokedAt = DateTime.UtcNow;

        await _UserManager.UpdateAsync(user);


        return true; 


    }
    private string RefreshTokenGenerated()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
