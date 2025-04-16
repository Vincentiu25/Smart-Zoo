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
public class ProfessionController(IProfessionService professionService, IUserService userService)
    : AuthorizedController(userService)
{
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<ProfessionDTO>>>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await professionService.GetAllAsync(currentUser.Result, cancellationToken))
            : ErrorMessageResult<List<ProfessionDTO>>(currentUser.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<ProfessionDTO>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await professionService.GetByIdAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult<ProfessionDTO>(currentUser.Error);
    }

    [HttpPost]
    public async Task<ActionResult<RequestResponse<Guid>>> Add([FromBody] ProfessionAddDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await professionService.AddAsync(dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult<Guid>(currentUser.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Update(Guid id, [FromBody] ProfessionUpdateDTO dto, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await professionService.UpdateAsync(id, dto, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null
            ? FromServiceResponse(await professionService.DeleteAsync(id, currentUser.Result, cancellationToken))
            : ErrorMessageResult(currentUser.Error);
    }
}
