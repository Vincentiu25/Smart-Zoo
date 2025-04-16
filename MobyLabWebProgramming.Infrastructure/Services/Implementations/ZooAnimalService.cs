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

public class ZooAnimalService : IZooAnimalService
{
    private readonly WebAppDatabaseContext _dbContext;
    private readonly IMailService _mailService;

    public ZooAnimalService(WebAppDatabaseContext dbContext, IMailService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
    }

    public async Task<ServiceResponse<List<ZooAnimalDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var animals = await _dbContext.ZooAnimals
            .Include(a => a.Species)
            .OrderBy(a => a.Species!.CommonName)
            .ThenBy(a => a.Name)
            .Select(a => new ZooAnimalDTO
            {
                Id = a.Id,
                Name = a.Name,
                Species = a.Species != null ? a.Species.CommonName : string.Empty,
                Age = a.Age,
                IsEndangered = a.IsEndangered
            })
            .ToListAsync(cancellationToken);

        return ServiceResponse.ForSuccess(animals);
    }

    public async Task<ServiceResponse<ZooAnimalDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var animal = await _dbContext.ZooAnimals
            .Include(a => a.Species)
            .Where(a => a.Id == id)
            .Select(a => new ZooAnimalDTO
            {
                Id = a.Id,
                Name = a.Name,
                Species = a.Species != null ? a.Species.CommonName : string.Empty,
                Age = a.Age,
                IsEndangered = a.IsEndangered
            })
            .FirstOrDefaultAsync(cancellationToken);

        return animal != null
            ? ServiceResponse.ForSuccess(animal)
            : ServiceResponse.FromError<ZooAnimalDTO>(new(HttpStatusCode.NotFound, "The animal was not found!", ErrorCodes.ZooAnimalNotFound));
    }

    public async Task<ServiceResponse<Guid>> AddAsync(ZooAnimalAddDTO animal, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Forbidden, "You are not allowed to add animals!", ErrorCodes.CannotAdd));
        }

        var entity = new ZooAnimal
        {
            Id = Guid.NewGuid(),
            Name = animal.Name,
            SpeciesId = animal.SpeciesId,
            Age = animal.Age,
            IsEndangered = animal.IsEndangered
        };

        _dbContext.ZooAnimals.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(entity).Reference(e => e.Species).LoadAsync(cancellationToken);

        //  Send notification email
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io",
            subject: "A new animal has been added",
            body: $"Animal '{entity.Name}' (Species: {entity.Species?.CommonName ?? "Unknown"}, Age: {entity.Age}) was added by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );

        return ServiceResponse.ForSuccess(entity.Id);
    }

    public async Task<ServiceResponse> UpdateAsync(ZooAnimalUpdateDTO animal, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not allowed to update animals!", ErrorCodes.CannotUpdate));
        }

        var entity = await _dbContext.ZooAnimals.FindAsync(animal.Id);

        if (entity == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The animal was not found!", ErrorCodes.ZooAnimalNotFound));
        }

        entity.Name = animal.Name;
        entity.SpeciesId = animal.SpeciesId;
        entity.Age = animal.Age;
        entity.IsEndangered = animal.IsEndangered;

        _dbContext.ZooAnimals.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only admin can delete animals!", ErrorCodes.CannotDelete));
        }

        var animal = await _dbContext.ZooAnimals
            .Include(a => a.Species)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (animal == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The animal was not found!", ErrorCodes.ZooAnimalNotFound));
        }

        _dbContext.ZooAnimals.Remove(animal);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(animal).Reference(e => e.Species).LoadAsync(cancellationToken);

        //  Send notification email
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io",
            subject: "An animal has been deleted",
            body: $"Animal '{animal.Name}' (Species: {animal.Species?.CommonName ?? "Unknown"}) was deleted by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );

        return ServiceResponse.ForSuccess();
    }
}
