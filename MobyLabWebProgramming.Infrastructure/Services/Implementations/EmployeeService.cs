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

public class EmployeeService : IEmployeeService
{
    private readonly WebAppDatabaseContext _dbContext;
    private readonly IMailService _mailService;

    public EmployeeService(WebAppDatabaseContext dbContext, IMailService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
    }

    public async Task<ServiceResponse<List<EmployeeDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await _dbContext.Employees
            .Include(e => e.Profession)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Age = e.Age,
                ProfessionName = e.Profession != null ? e.Profession.Name : string.Empty
            })
            .ToListAsync(cancellationToken));
    }

    public async Task<ServiceResponse<EmployeeDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .Include(e => e.Profession)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Age = e.Age,
                ProfessionName = e.Profession != null ? e.Profession.Name : string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);

        return employee != null
            ? ServiceResponse.ForSuccess(employee)
            : ServiceResponse.FromError<EmployeeDTO>(new(HttpStatusCode.NotFound, "The employee was not found.", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse<Guid>> AddAsync(EmployeeAddDTO employee, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError<Guid>(new(HttpStatusCode.Forbidden, "You are not allowed to add an employee!", ErrorCodes.CannotAdd));

        var entity = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = employee.FullName,
            Age = employee.Age,
            ProfessionId = employee.ProfessionId
        };

        _dbContext.Employees.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(entity).Reference(e => e.Profession).LoadAsync(cancellationToken);

        // Trimitem email de notificare
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io",
            subject: "A new employee has been added",
            body: $"Employee '{entity.FullName}' (Age {entity.Age}, Profession: {entity.Profession?.Name ?? "Unknown"}) was added by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );

        return ServiceResponse.ForSuccess(entity.Id);
    }

    public async Task<ServiceResponse> UpdateAsync(Guid id, EmployeeUpdateDTO employee, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not allowed to update an employee!", ErrorCodes.CannotUpdate));

        var entity = await _dbContext.Employees.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The employee was not found!", ErrorCodes.EntityNotFound));

        entity.FullName = employee.FullName;
        entity.Age = employee.Age;
        entity.ProfessionId = employee.ProfessionId;

        _dbContext.Employees.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only admin can delete an employee!", ErrorCodes.CannotDelete));

        var entity = await _dbContext.Employees.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The employee was not found for deletion!", ErrorCodes.EntityNotFound));

        _dbContext.Employees.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(entity).Reference(e => e.Profession).LoadAsync(cancellationToken);

        //  Email notificare pentru ștergere
        await _mailService.SendMail(
            recipientEmail: "9d06306b7d-5d4640@inbox.mailtrap.io",
            subject: "An employee has been deleted",
            body: $"Employee '{entity.FullName}' (Age {entity.Age}, Profession: {entity.Profession?.Name ?? "Unknown"}) was deleted by {requestingUser.Name}.",
            isHtmlBody: false,
            senderTitle: "Zoo System",
            cancellationToken: cancellationToken
        );

        return ServiceResponse.ForSuccess();
    }
}
