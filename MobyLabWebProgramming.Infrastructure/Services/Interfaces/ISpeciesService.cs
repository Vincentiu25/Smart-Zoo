using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ISpeciesService
{
    Task<ServiceResponse<List<SpeciesDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<SpeciesDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddAsync(SpeciesAddDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(Guid id, SpeciesUpdateDTO dto, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
