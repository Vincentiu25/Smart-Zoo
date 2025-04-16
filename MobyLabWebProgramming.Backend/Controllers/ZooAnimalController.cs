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
public class ZooAnimalController(IZooAnimalService zooAnimalService, IUserService userService)
    : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<ZooAnimalDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await zooAnimalService.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<ZooAnimalDTO>>(currentUser.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<ZooAnimalDTO>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await zooAnimalService.GetByIdAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult<ZooAnimalDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse<Guid>>> Add([FromBody] ZooAnimalAddDTO animal, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await zooAnimalService.AddAsync(animal, currentUser.Result, cancellationToken))
            : ErrorMessageResult<Guid>(currentUser.Error);
    }

    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] ZooAnimalUpdateDTO animal, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await zooAnimalService.UpdateAsync(animal, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await zooAnimalService.DeleteAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }
}
