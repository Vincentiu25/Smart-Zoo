using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IEmployeeZooAnimalService
{
    Task<ServiceResponse<List<EmployeeZooAnimalDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<EmployeeZooAnimalDTO>> GetByIdsAsync(Guid employeeId, Guid zooAnimalId, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> AddAsync(EmployeeZooAnimalAddDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(EmployeeZooAnimalDeleteDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
