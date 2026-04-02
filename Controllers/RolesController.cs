using BucketSurvey.Api.Contract.Roles;

namespace BucketSurvey.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _RoleService = roleService;

    [HttpGet]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool hasincluded , CancellationToken cancellationToken)
    {
       var result = await _RoleService.GetAllAsync(cancellationToken , hasincluded);

        return Ok(result.Value);
    }

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string Id )
    {
        var result = await _RoleService.GetAsync(Id);

        return result.IsSuccess ? Ok(result.Value): result.ToProblem();
    }

    [HttpPost()]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> Add(RoleRequest request , CancellationToken cancellationToken)
    {
        var result = await _RoleService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value): result.ToProblem();
    }

    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update([FromRoute]string Id , RoleRequest request , CancellationToken cancellationToken)
    {
        var result = await _RoleService.UpdateAsync(Id , request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{Id}/toggle-status")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string Id)
    {
        var result = await _RoleService.ToggleStatusAsync(Id);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }




}
