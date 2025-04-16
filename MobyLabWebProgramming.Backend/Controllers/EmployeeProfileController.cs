using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class EmployeeProfileController(IEmployeeProfileService service, IUserService userService) : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<EmployeeProfileDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<EmployeeProfileDTO>>(currentUser.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<EmployeeProfileDTO>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.GetByIdAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult<EmployeeProfileDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse<Guid>>> Add([FromBody] EmployeeProfileAddDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.AddAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult<Guid>(currentUser.Error);
    }

    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] EmployeeProfileUpdateDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.UpdateAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.DeleteAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }
}
