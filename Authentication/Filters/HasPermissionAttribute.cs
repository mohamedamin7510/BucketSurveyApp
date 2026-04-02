using System.Security;

namespace BucketSurvey.Api.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission) 
{

}
