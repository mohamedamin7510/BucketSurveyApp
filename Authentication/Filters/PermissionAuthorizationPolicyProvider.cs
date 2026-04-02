namespace BucketSurvey.Api.Authentication.Filters;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> Options) : DefaultAuthorizationPolicyProvider(Options)
{
    private readonly AuthorizationOptions _Options = Options.Value;
    public async override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var Policy = await base.GetPolicyAsync(policyName);

        if (Policy is not null)
            return Policy;

        var newpolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName)).Build();

        _Options.AddPolicy(policyName, newpolicy);

        return newpolicy;
    }

}
