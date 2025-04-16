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

public class EmployeeZooAnimalService(WebAppDatabaseContext dbContext) : IEmployeeZooAnimalService
{
    public async Task<ServiceResponse<List<EmployeeZooAnimalDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await dbContext.EmployeeZooAnimals
            .Include(eza => eza.Employee).ThenInclude(e => e.Profession)
            .Include(eza => eza.ZooAnimal).ThenInclude(z => z.Species)
            .Select(eza => new EmployeeZooAnimalDTO
            {
                EmployeeId = eza.EmployeeId,
                EmployeeName = eza.Employee.FullName,
                ProfessionName = eza.Employee.Profession != null ? eza.Employee.Profession.Name : string.Empty,
                ZooAnimalId = eza.ZooAnimalId,
                AnimalName = eza.ZooAnimal.Name,
                SpeciesName = eza.ZooAnimal.Species != null ? eza.ZooAnimal.Species.CommonName : string.Empty
            })
            .ToListAsync(cancellationToken));
    }

    public async Task<ServiceResponse<EmployeeZooAnimalDTO>> GetByIdsAsync(Guid employeeId, Guid zooAnimalId, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.EmployeeZooAnimals
            .Include(eza => eza.Employee).ThenInclude(e => e.Profession)
            .Include(eza => eza.ZooAnimal).ThenInclude(z => z.Species)
            .FirstOrDefaultAsync(eza => eza.EmployeeId == employeeId && eza.ZooAnimalId == zooAnimalId, cancellationToken);

        return entity != null
            ? ServiceResponse.ForSuccess(new EmployeeZooAnimalDTO
            {
                EmployeeId = entity.EmployeeId,
                EmployeeName = entity.Employee.FullName,
                ProfessionName = entity.Employee.Profession?.Name ?? string.Empty,
                ZooAnimalId = entity.ZooAnimalId,
                AnimalName = entity.ZooAnimal.Name,
                SpeciesName = entity.ZooAnimal.Species?.CommonName ?? string.Empty
            })
            : ServiceResponse.FromError<EmployeeZooAnimalDTO>(new(HttpStatusCode.NotFound, "The relationship was not found!", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse> AddAsync(EmployeeZooAnimalAddDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not allowed to assign animals!", ErrorCodes.CannotAdd));

        var employeeExists = await dbContext.Employees.AnyAsync(e => e.Id == dto.EmployeeId, cancellationToken);
        var animalExists = await dbContext.ZooAnimals.AnyAsync(a => a.Id == dto.ZooAnimalId, cancellationToken);
        var alreadyExists = await dbContext.EmployeeZooAnimals.AnyAsync(eza => eza.EmployeeId == dto.EmployeeId && eza.ZooAnimalId == dto.ZooAnimalId, cancellationToken);

        if (!employeeExists || !animalExists)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The employee or animal was not found!", ErrorCodes.EntityNotFound));

        if (alreadyExists)
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "This relationship already exists!", ErrorCodes.CannotAdd));

        dbContext.EmployeeZooAnimals.Add(new EmployeeZooAnimal
        {
            EmployeeId = dto.EmployeeId,
            ZooAnimalId = dto.ZooAnimalId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(EmployeeZooAnimalDeleteDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only admin can delete relationships!", ErrorCodes.CannotDelete));

        var entity = await dbContext.EmployeeZooAnimals
            .FirstOrDefaultAsync(eza => eza.EmployeeId == dto.EmployeeId && eza.ZooAnimalId == dto.ZooAnimalId, cancellationToken);

        if (entity == null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The relationship was not found!", ErrorCodes.EntityNotFound));

        dbContext.EmployeeZooAnimals.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess();
    }
}
