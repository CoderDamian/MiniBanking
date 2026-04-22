using Mini_Banking.Application.DTOs;

namespace Mini_Banking.Application.Contracts
{
    public interface IBankTransactionService
    {
        Task<string> DepositAsync(CreateDepositDTO depositDTO, string key, string requestHash, CancellationToken cancellationToken = default);
        Task<string> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO, CancellationToken cancellationToken = default);
        //Guid Transfer(AccountDTO receiver, AccountDTO sender, decimal amount);
    }
}
