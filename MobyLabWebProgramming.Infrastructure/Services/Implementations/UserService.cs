using System.Net;
using MobyLabWebProgramming.Core.Constants;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class UserService(
    IRepository<WebAppDatabaseContext> repository,
    ILoginService loginService,
    IMailService mailService,
    ILogger<UserService> logger) : IUserService
{
public async Task<ServiceResponse<UserDTO>> GetUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
{
    if (requestingUser == null)
    {
        return ServiceResponse.FromError<UserDTO>(
            new(HttpStatusCode.Forbidden, "You are not allowed to view this user!", ErrorCodes.CannotRead)
        );
    }

    // Admin și Personnel pot vedea orice, Client doar pe sine
    if (requestingUser.Role == UserRoleEnum.Client && requestingUser.Id != id)
    {
        return ServiceResponse.FromError<UserDTO>(
            new(HttpStatusCode.Forbidden, "Clients can only view their own information!", ErrorCodes.CannotRead)
        );
    }

    var result = await repository.GetAsync(new UserProjectionSpec(id), cancellationToken);

    return result != null
        ? ServiceResponse.ForSuccess(result)
        : ServiceResponse.FromError<UserDTO>(CommonErrors.UserNotFound);
}



public async Task<ServiceResponse<PagedResponse<UserDTO>>> GetUsers(
    PaginationSearchQueryParams pagination,
    UserDTO? requestingUser = null,
    CancellationToken cancellationToken = default)
{
    
    logger.LogInformation("[GET USERS] Requested by: {Email}, Role: {Role}", requestingUser?.Email, requestingUser?.Role);



    if (requestingUser == null || (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Role != UserRoleEnum.Personnel))
    {
        return ServiceResponse.FromError<PagedResponse<UserDTO>>(
            new(HttpStatusCode.Forbidden, "Only admin and personnel can view users!", ErrorCodes.CannotRead)
        );
    }

    var result = await repository.PageAsync(pagination, new UserProjectionSpec(pagination.Search), cancellationToken);
    return ServiceResponse.ForSuccess(result);
}

    public async Task<ServiceResponse<LoginResponseDTO>> Login(LoginDTO login, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserSpec(login.Email), cancellationToken);
        File.WriteAllText("test-login.txt", $"Email: {login.Email}, Parola: {login.Password}, Ora: {DateTime.Now}");

        if (result == null)
        {
            logger.LogWarning("[LOGIN] User not found for email: {Email}", login.Email);
            return ServiceResponse.FromError<LoginResponseDTO>(CommonErrors.UserNotFound);
        }

        File.AppendAllText("login-debug.txt",
            $"Comparing '{login.Password}' with '{result.Password}' at {DateTime.Now}\n");

        if (result.Password != login.Password)
        {
            logger.LogWarning("[LOGIN] Parola nu se potrivește pentru userul {Email}", login.Email);
            return ServiceResponse.FromError<LoginResponseDTO>(
                new(HttpStatusCode.BadRequest, "Wrong password!", ErrorCodes.WrongPassword)
            );
        }

        var user = new UserDTO
        {
            Id = result.Id,
            Email = result.Email,
            Name = result.Name,
            Role = result.Role
        };

        logger.LogInformation("[LOGIN] Autentificare reușită pentru: {Email}", user.Email);

        return ServiceResponse.ForSuccess(new LoginResponseDTO
        {
            User = user,
            Token = loginService.GetToken(user, DateTime.UtcNow, new(7, 0, 0, 0))
        });
    }

    public async Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default) =>
        ServiceResponse.ForSuccess(await repository.GetCountAsync<User>(cancellationToken));

    public async Task<ServiceResponse> AddUser(UserAddDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser is not null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(
                new(HttpStatusCode.Forbidden, "Only the admin can add users!", ErrorCodes.CannotAdd)
            );
        }

        var result = await repository.GetAsync(new UserSpec(user.Email), cancellationToken);

        if (result != null)
        {
            return ServiceResponse.FromError(
                new(HttpStatusCode.Conflict, "The user already exists!", ErrorCodes.UserAlreadyExists)
            );
        }

        await repository.AddAsync(new User
        {
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Password = user.Password
        }, cancellationToken);

        await mailService.SendMail(
            user.Email,
            "Welcome!",
            MailTemplates.UserAddTemplate(user.Name),
            true,
            "My App",
            cancellationToken
        );

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateUser(UserUpdateDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser is null || (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != user.Id))
        {
            return ServiceResponse.FromError(
                new(HttpStatusCode.Forbidden, "Only the admin or the user themselves can update this user!", ErrorCodes.CannotUpdate)
            );
        }

        var entity = await repository.GetAsync(new UserSpec(user.Id), cancellationToken);

        if (entity != null)
        {
            entity.Name = user.Name ?? entity.Name;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                entity.Password = user.Password;
            }

            await repository.UpdateAsync(entity, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
    {
        if (requestingUser is null || (requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != id))
        {
            return ServiceResponse.FromError(
                new(HttpStatusCode.Forbidden, "Only the admin or the own user can delete the user!", ErrorCodes.CannotDelete)
            );
        }

        await repository.DeleteAsync<User>(id, cancellationToken);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<UserDTO>> GetUserWithoutPermissions(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserProjectionSpec(id), cancellationToken);

        return result != null
            ? ServiceResponse.ForSuccess(result)
            : ServiceResponse.FromError<UserDTO>(CommonErrors.UserNotFound);
    }

}
