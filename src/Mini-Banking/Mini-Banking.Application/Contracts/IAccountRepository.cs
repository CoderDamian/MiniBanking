using Mini_Banking.Domain.Entities;

namespace Mini_Banking.Application.Contracts
{
    public interface IAccountRepository
    {
        Task<Account?> GetByAsync(int ID, CancellationToken cancellationToken = default);
        Task UpdateBalanceAsync(Account account, CancellationToken cancellationToken = default);
    }
}
