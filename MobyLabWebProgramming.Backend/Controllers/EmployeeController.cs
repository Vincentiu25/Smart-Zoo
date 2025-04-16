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
public class EmployeeController(IEmployeeService employeeService, IUserService userService)
    : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<EmployeeDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await employeeService.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<EmployeeDTO>>(currentUser.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<EmployeeDTO>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await employeeService.GetByIdAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult<EmployeeDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse<Guid>>> Add([FromBody] EmployeeAddDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await employeeService.AddAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult<Guid>(currentUser.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Update(Guid id, [FromBody] EmployeeUpdateDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await employeeService.UpdateAsync(id, dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await employeeService.DeleteAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }
}
