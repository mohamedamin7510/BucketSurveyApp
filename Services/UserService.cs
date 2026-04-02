 using BucketSurvey.Api.Contract.User;
using System.Runtime.InteropServices;

namespace BucketSurvey.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager , ApplicationContext applicationUser , RoleManager<ApplicationRole> roleManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _UserManager = userManager;
    private readonly ApplicationContext _Context = applicationUser;


    public async Task<Result<ProfileResponse>> GetUserAsync(string Id)
    {
        var Response = await _UserManager.Users
            .Where(x=>x.Id == Id)
            .ProjectToType<ProfileResponse>()
            .SingleAsync();

        return Result.Success(Response);

    }

    public async Task  UpdateUserAsync(string Id, ProfileRequest request)
    {
    
        _UserManager.Users
            .Where(x => x.Id == Id)
           .ExecuteUpdate(
            setPropertyCalls: setter => setter
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(y => y.LastName, request.LastName)
            );

    }

    public async Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request)
    {
        var user = await  _UserManager.FindByIdAsync(Id);

        var result = await _UserManager.ChangePasswordAsync(user! , request.CurrentPassword , request.NewPassword);

        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Faluire(new Error(error.Code , error.Description , StatusCodes.Status400BadRequest));
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken cancelationToken)
    {
        var response = await ( from u in _Context.Users 
                               join ur in _Context.UserRoles
                               on u.Id equals ur.UserId
                               join r in _Context.Roles
                               on ur.RoleId equals r.Id into roles
                               where !roles.Any(x => x.Name == DefaultRoles.Member)
                               select  new
                               {
                                   u.Id,
                                   u.FirstName,
                                   u.LastName,
                                   u.Email,
                                   u.IsDisabled,
                                   roles
                               }
                               )
                               .GroupBy(
                                    x =>
                                    new { x.Id, x.FirstName, x.LastName, x.Email, x.IsDisabled }
                               )
                               .Select(x => new UserResponse
                               (
                                   x.Key.Id,
                                   x.Key.FirstName,
                                   x.Key.LastName,
                                   x.Key.Email,
                                   x.Key.IsDisabled,
                                   x.SelectMany(x => x.roles.Select(r => r.Name!)).ToList()
                                   
                               ))
                               .ToListAsync(cancelationToken);

        return Result.Success<IEnumerable<UserResponse>>(response);

    }

    public async Task<Result<UserResponse>> GetUserAsync(string id, CancellationToken cancelationToken)
    {

        if (await _UserManager.FindByIdAsync(id) is not { } user)
            return Result.Faluire<UserResponse>(UserError.UserNotFound);

        var roles = await _UserManager.GetRolesAsync(user!);

        var response = (user, roles).Adapt<UserResponse>();


        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddUserAsync( UserRequest request, CancellationToken cancelationToken)
    {

       var IsUserexists = await _UserManager.Users.AnyAsync(x => x.Email == request.Email, cancelationToken);

        if (IsUserexists)
            return Result.Faluire<UserResponse>(UserError.DuplicatedEmail);

        var ValidRoles = await _Context.Roles
            .Select(x => x.Name)
            .ToListAsync(cancelationToken);


        if ( request.Roles.Except(ValidRoles).Any())
        {
            return Result.Faluire<UserResponse>(RoleErrors.InvalidRole);
        }

        var user = request.Adapt<ApplicationUser>();

       var createduser =  await _UserManager.CreateAsync(user , request.Password);

        IdentityResult? CreatedRoles = null;

        if (createduser.Succeeded)
        {
             CreatedRoles  = await  _UserManager.AddToRolesAsync(user , request.Roles);

            var response = (user , request.Roles).Adapt<UserResponse>();

            if (CreatedRoles!.Succeeded)
                return Result.Success(response);
        }  


        var error = createduser.Errors.First();

        return Result.Faluire<UserResponse>(new Error(error.Code , error.Description , StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateUserAsync(string UserId , UpdateUserRequest request, CancellationToken cancelationToken)
    {
        var User = await _UserManager.FindByIdAsync(UserId);

        if (User is null)
            return Result.Faluire<UserResponse>(UserError.UserNotFound);

        var IsEmailDuplicated = await _UserManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != User!.Id , cancelationToken);

        if (IsEmailDuplicated)
            return Result.Faluire<UserResponse>(UserError.DuplicatedEmail);

        User.FirstName = request.FirstName;
        User.LastName = request.LastName;
        User.Email = request.Email;
        User.UserName = request.Email;
         

        var result = await _UserManager.UpdateAsync(User);


        if (result.Succeeded)
        {

            var ValidRoles = await _Context.Roles
                .Select(x => x.Name)
                .ToListAsync(cancelationToken);

            if (request.Roles.Except(ValidRoles).Any())
            {
                return Result.Faluire<UserResponse>(RoleErrors.InvalidRole);
            }

            var CurrentRoles = _UserManager.GetRolesAsync(User).Result;

            var newRoles = request.Roles.Except(CurrentRoles);

            var removedRoles = CurrentRoles.Except(request.Roles);

            var addingRoles = await _UserManager.AddToRolesAsync(User, newRoles);

            if (addingRoles.Succeeded)
            {
                await _UserManager.RemoveFromRolesAsync(User, removedRoles);
            }

            var response = (User , request.Roles).Adapt<UserResponse>();

            return Result.Success();

        }


        var error = result.Errors.First();

        return Result.Faluire(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ToggleStatusAsync(string UserId ,  CancellationToken cancelationToken)
    {
        var User = await _UserManager.FindByIdAsync(UserId);

        if (User is null)
            return Result.Faluire<UserResponse>(UserError.UserNotFound);

        User.IsDisabled = !User.IsDisabled;

        await _Context.SaveChangesAsync(cancelationToken);

        return Result.Success();
 
    }

    public async Task<Result> UnlockUserAsync(string UserId, CancellationToken cancelationToken)
    {


      if( await _UserManager.FindByIdAsync(UserId) is not {} user)
            return Result.Faluire<UserResponse>(UserError.UserNotFound);

        await _UserManager.SetLockoutEndDateAsync(user, null);

        return Result.Success();
    }

}
