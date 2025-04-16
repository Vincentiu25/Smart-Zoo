using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IEmployeeService
{
    Task<ServiceResponse<List<EmployeeDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<EmployeeDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddAsync(EmployeeAddDTO employee, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(Guid id, EmployeeUpdateDTO employee, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
