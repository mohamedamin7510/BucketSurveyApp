namespace BucketSurvey.Api.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User.Identity;



        if (!user.IsAuthenticated || user is null)
            return;

        if (!context.User.Claims.Any(x => x.Type == Permissions.Type && x.Value == requirement.Permission))
            return;
       

      //   context.Succeed(requirement);


        //if (user is not { IsAuthenticated: true }
        //|| !context.User.Claims.Any(x => x.Type == Permissions.Type && x.Value == requirement.Permission))
        //    return; 

        context.Succeed(requirement);
    }
}
