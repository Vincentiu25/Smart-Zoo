using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IEmployeeProfileService
{
    Task<ServiceResponse<List<EmployeeProfileDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<EmployeeProfileDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddAsync(EmployeeProfileAddDTO profile, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(EmployeeProfileUpdateDTO profile, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
