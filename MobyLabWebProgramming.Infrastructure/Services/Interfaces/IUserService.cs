using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be used to manage user information.
/// Most routes and business logic will need to know what user is currently using the backend.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// GetUser will provide the information about a user given its user Id.
    /// It also checks if the requesting user has permission to access the user info.
    /// </summary>
    public Task<ServiceResponse<UserDTO>> GetUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GetUsers returns a page with user information from the database.
    /// It checks if the requesting user is an admin.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<UserDTO>>> GetUsers(PaginationSearchQueryParams pagination, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login responds to a user login request with the JWT token and user information.
    /// </summary>
    public Task<ServiceResponse<LoginResponseDTO>> Login(LoginDTO login, CancellationToken cancellationToken = default);

    /// <summary>
    /// GetUserCount returns the number of users in the database.
    /// </summary>
    public Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default);

    /// <summary>
    /// AddUser adds a user and verifies if requesting user has permissions to add one.
    /// If the requesting user is null then no verification is performed (e.g. app start).
    /// </summary>
    public Task<ServiceResponse> AddUser(UserAddDTO user, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// UpdateUser updates a user and verifies if requesting user has permissions to update it.
    /// If the user is their own account, that should be allowed.
    /// </summary>
    public Task<ServiceResponse> UpdateUser(UserUpdateDTO user, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// DeleteUser deletes a user and verifies if requesting user has permissions to delete it.
    /// If the user is their own account, that should be allowed.
    /// </summary>
    public Task<ServiceResponse> DeleteUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    public Task<ServiceResponse<UserDTO>> GetUserWithoutPermissions(Guid id, CancellationToken cancellationToken = default);

}
