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
public class SpeciesController(ISpeciesService speciesService, IUserService userService)
    : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<SpeciesDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await speciesService.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<SpeciesDTO>>(currentUser.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<SpeciesDTO>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await speciesService.GetByIdAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult<SpeciesDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse<Guid>>> Add([FromBody] SpeciesAddDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await speciesService.AddAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult<Guid>(currentUser.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Update(Guid id, [FromBody] SpeciesUpdateDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await speciesService.UpdateAsync(id, dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await speciesService.DeleteAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    
}
