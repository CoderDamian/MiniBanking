using Mini_Banking.Domain.Entities;

namespace Mini_Banking.Application.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserBy(int id, CancellationToken cancellationToken = default);
        Task<Func<int>> AddUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
