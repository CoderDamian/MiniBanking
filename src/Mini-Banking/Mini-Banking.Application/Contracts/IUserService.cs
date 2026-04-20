using Mini_Banking.Application.DTOs;

namespace Mini_Banking.Application.Contracts
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDTO user, CancellationToken cancellationToken = default);
    }
}
