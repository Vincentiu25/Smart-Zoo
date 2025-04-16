using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IZooAnimalService
{
    Task<ServiceResponse<List<ZooAnimalDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<ZooAnimalDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddAsync(ZooAnimalAddDTO animal, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(ZooAnimalUpdateDTO animal, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
