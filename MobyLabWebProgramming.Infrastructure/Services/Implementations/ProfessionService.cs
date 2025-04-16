using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Net;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class ProfessionService(WebAppDatabaseContext dbContext) : IProfessionService
{
    public async Task<ServiceResponse<List<ProfessionDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await dbContext.Professions
            .Select(p => new ProfessionDTO { Id = p.Id, Name = p.Name })
            .ToListAsync(cancellationToken));
    }

    public async Task<ServiceResponse<ProfessionDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var profession = await dbContext.Professions
            .Where(p => p.Id == id)
            .Select(p => new ProfessionDTO { Id = p.Id, Name = p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        return profession != null
            ? ServiceResponse.ForSuccess(profession)
            : ServiceResponse.FromError<ProfessionDTO>(new(HttpStatusCode.NotFound, "Profesia nu a fost găsită!", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse<Guid>> AddAsync(ProfessionAddDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Forbidden, "Doar adminul sau personalul pot adăuga profesii!", ErrorCodes.CannotAdd));

        var exists = await dbContext.Professions.AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower(), cancellationToken);

        if (exists)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Conflict, "Există deja o profesie cu acest nume!", ErrorCodes.CannotAdd));

        var profession = new Profession { Id = Guid.NewGuid(), Name = dto.Name };
        dbContext.Professions.Add(profession);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse.ForSuccess(profession.Id);
    }

    public async Task<ServiceResponse> UpdateAsync(Guid id, ProfessionUpdateDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Doar adminul sau personalul pot modifica profesii!", ErrorCodes.CannotUpdate));

        var entity = await dbContext.Professions.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "Profesia nu a fost găsită!", ErrorCodes.EntityNotFound));

        var duplicate = await dbContext.Professions.AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id, cancellationToken);
        if (duplicate)
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "Altă profesie cu acest nume există deja!", ErrorCodes.CannotUpdate));

        entity.Name = dto.Name;
        dbContext.Professions.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Doar adminul poate șterge profesii!", ErrorCodes.CannotDelete));

        var entity = await dbContext.Professions.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "Profesia nu a fost găsită pentru ștergere!", ErrorCodes.EntityNotFound));

        dbContext.Professions.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}
