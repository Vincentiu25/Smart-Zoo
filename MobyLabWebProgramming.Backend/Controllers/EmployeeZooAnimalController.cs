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
public class AnimalAssignmentsController(IEmployeeZooAnimalService service, IUserService userService) : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<EmployeeZooAnimalDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<EmployeeZooAnimalDTO>>(currentUser.Error);
    }

    [HttpGet("{employeeId:guid}/{zooAnimalId:guid}")]
    public async Task<ActionResult<RequestResponse<EmployeeZooAnimalDTO>>> GetByIds(Guid employeeId, Guid zooAnimalId, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.GetByIdsAsync(employeeId, zooAnimalId, currentUser.Result, cancellationToken))
            : ErrorMessageResult<EmployeeZooAnimalDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] EmployeeZooAnimalAddDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.AddAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete]
    public async Task<ActionResult<RequestResponse>> Delete([FromBody] EmployeeZooAnimalDeleteDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await service.DeleteAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }
}
