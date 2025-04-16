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

public class SpeciesService : ISpeciesService
{
    private readonly WebAppDatabaseContext _dbContext;
    private readonly IMailService _mailService;

    public SpeciesService(WebAppDatabaseContext dbContext, IMailService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
    }

    public async Task<ServiceResponse<List<SpeciesDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await _dbContext.Species
            .Select(s => new SpeciesDTO
            {
                Id = s.Id,
                CommonName = s.CommonName,
                ScientificName = s.ScientificName,
                Habitat = s.Habitat,
                Diet = s.Diet
            })
            .ToListAsync(cancellationToken));
    }

    public async Task<ServiceResponse<SpeciesDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var species = await _dbContext.Species
            .Where(s => s.Id == id)
            .Select(s => new SpeciesDTO
            {
                Id = s.Id,
                CommonName = s.CommonName,
                ScientificName = s.ScientificName,
                Habitat = s.Habitat,
                Diet = s.Diet
            })
            .FirstOrDefaultAsync(cancellationToken);

        return species != null
            ? ServiceResponse.ForSuccess(species)
            : ServiceResponse.FromError<SpeciesDTO>(new(HttpStatusCode.NotFound, "The species was not found.", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse<Guid>> AddAsync(SpeciesAddDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Forbidden, "You are not allowed to add a species!", ErrorCodes.CannotAdd));
        }

        var exists = await _dbContext.Species.AnyAsync(s =>
            s.CommonName.ToLower() == dto.CommonName.ToLower() ||
            s.ScientificName.ToLower() == dto.ScientificName.ToLower(),
            cancellationToken);

        if (exists)
        {
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Conflict, "The species already exists!", ErrorCodes.CannotAdd));
        }

        var species = new Species
        {
            Id = Guid.NewGuid(),
            CommonName = dto.CommonName,
            ScientificName = dto.ScientificName,
            Habitat = dto.Habitat,
            Diet = dto.Diet
        };

        _dbContext.Species.Add(species);
        await _dbContext.SaveChangesAsync(cancellationToken);

        //  Trimitem email de notificare către admin
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io", 
            subject: "A new species has been added",
            body: $"Species '{dto.CommonName}' ({dto.ScientificName}) was added by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );

        return ServiceResponse.ForSuccess(species.Id);
    }

    public async Task<ServiceResponse> UpdateAsync(Guid id, SpeciesUpdateDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not allowed to update a species!", ErrorCodes.CannotUpdate));
        }

        var species = await _dbContext.Species.FindAsync(new object[] { id }, cancellationToken);

        if (species == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The species was not found!", ErrorCodes.EntityNotFound));
        }

        species.CommonName = dto.CommonName ?? species.CommonName;
        species.ScientificName = dto.ScientificName ?? species.ScientificName;
        species.Habitat = dto.Habitat ?? species.Habitat;
        species.Diet = dto.Diet ?? species.Diet;

        _dbContext.Species.Update(species);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only admin can delete a species!", ErrorCodes.CannotDelete));
        }

        var species = await _dbContext.Species.FindAsync(new object[] { id }, cancellationToken);

        if (species == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The species was not found for deletion!", ErrorCodes.EntityNotFound));
        }

        _dbContext.Species.Remove(species);
        await _dbContext.SaveChangesAsync(cancellationToken);

        //  Trimitem email de notificare către admin
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io",
            subject: "A species has been deleted",
            body: $"Species '{species.CommonName}' ({species.ScientificName}) was deleted by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );


        return ServiceResponse.ForSuccess();
    }
}
