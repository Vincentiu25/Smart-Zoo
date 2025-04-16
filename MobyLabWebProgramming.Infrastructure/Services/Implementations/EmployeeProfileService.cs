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

public class EmployeeProfileService(WebAppDatabaseContext dbContext) : IEmployeeProfileService
{
    public async Task<ServiceResponse<List<EmployeeProfileDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await dbContext.EmployeeProfiles
            .Include(p => p.Employee)
                .ThenInclude(e => e.Profession)
            .Include(p => p.Employee)
                .ThenInclude(e => e.EmployeeZooAnimals)
                    .ThenInclude(eza => eza.ZooAnimal)
                        .ThenInclude(za => za.Species)
            .Select(p => new EmployeeProfileDTO
            {
                Id = p.Id,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FullName,
                ProfessionName = p.Employee.Profession.Name,
                ZooAnimals = p.Employee.EmployeeZooAnimals
                    .Select(eza => $"{eza.ZooAnimal.Name} ({eza.ZooAnimal.Species!.CommonName})")
                    .ToList()
            })
            .ToListAsync(cancellationToken));
    }

    public async Task<ServiceResponse<EmployeeProfileDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.EmployeeProfiles
            .Include(p => p.Employee)
                .ThenInclude(e => e.Profession)
            .Include(p => p.Employee)
                .ThenInclude(e => e.EmployeeZooAnimals)
                    .ThenInclude(eza => eza.ZooAnimal)
                        .ThenInclude(za => za.Species)
            .Where(p => p.Id == id)
            .Select(p => new EmployeeProfileDTO
            {
                Id = p.Id,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FullName,
                ProfessionName = p.Employee.Profession.Name,
                ZooAnimals = p.Employee.EmployeeZooAnimals
                    .Select(eza => $"{eza.ZooAnimal.Name} ({eza.ZooAnimal.Species!.CommonName})")
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return profile != null
            ? ServiceResponse.ForSuccess(profile)
            : ServiceResponse.FromError<EmployeeProfileDTO>(new(HttpStatusCode.NotFound, "The profile was not found!", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse<Guid>> AddAsync(EmployeeProfileAddDTO profile, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Forbidden, "You are not allowed to add a profile!", ErrorCodes.CannotAdd));

        var emailExists = await dbContext.EmployeeProfiles.AnyAsync(p => p.Email.ToLower() == profile.Email.ToLower(), cancellationToken);
        if (emailExists)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Conflict, "Email already in use!", ErrorCodes.CannotAdd));

        var phoneExists = await dbContext.EmployeeProfiles.AnyAsync(p => p.PhoneNumber == profile.PhoneNumber, cancellationToken);
        if (phoneExists)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Conflict, "Phone number already in use!", ErrorCodes.CannotAdd));

        var entity = new EmployeeProfile
        {
            Id = Guid.NewGuid(),
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            EmployeeId = profile.EmployeeId
        };

        dbContext.EmployeeProfiles.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess(entity.Id);
    }

    public async Task<ServiceResponse> UpdateAsync(EmployeeProfileUpdateDTO profile, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not allowed to update a profile!", ErrorCodes.CannotUpdate));

        var entity = await dbContext.EmployeeProfiles.FirstOrDefaultAsync(p => p.Id == profile.Id, cancellationToken);
        if (entity is null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The profile was not found for update!", ErrorCodes.EntityNotFound));

        var emailDuplicate = await dbContext.EmployeeProfiles.AnyAsync(p => p.Email.ToLower() == profile.Email.ToLower() && p.Id != profile.Id, cancellationToken);
        if (emailDuplicate)
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "Another profile with this email already exists!", ErrorCodes.CannotUpdate));

        var phoneDuplicate = await dbContext.EmployeeProfiles.AnyAsync(p => p.PhoneNumber == profile.PhoneNumber && p.Id != profile.Id, cancellationToken);
        if (phoneDuplicate)
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "Another profile with this phone number already exists!", ErrorCodes.CannotUpdate));

        entity.Email = profile.Email;
        entity.PhoneNumber = profile.PhoneNumber;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only admin can delete a profile!", ErrorCodes.CannotDelete));

        var entity = await dbContext.EmployeeProfiles.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The profile was not found for deletion!", ErrorCodes.EntityNotFound));

        dbContext.EmployeeProfiles.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess();
    }
}
