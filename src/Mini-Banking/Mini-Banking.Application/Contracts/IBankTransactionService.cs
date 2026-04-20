using Mini_Banking.Application.DTOs;

namespace Mini_Banking.Application.Contracts
{
    public interface IBankTransactionService
    {
        Task<Guid> DepositAsync(CreateDepositDTO depositDTO, CancellationToken cancellationToken = default);
        Task<Guid> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO, CancellationToken cancellationToken = default);
        //Guid Transfer(AccountDTO receiver, AccountDTO sender, decimal amount);
    }
}
