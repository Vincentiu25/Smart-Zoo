using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IProfessionService
{
    Task<ServiceResponse<List<ProfessionDTO>>> GetAllAsync(UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<ProfessionDTO>> GetByIdAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddAsync(ProfessionAddDTO profession, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(Guid id, ProfessionUpdateDTO profession, UserDTO requestingUser, CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAsync(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default);
}
