using Mini_Banking.Application.DTOs;

namespace Mini_Banking.Application.Contracts
{
    public interface IBankTransactionService
    {
        Task<CreateDepositResponse> DepositAsync(CreateDepositRequest depositDTO,
                                                 string key,
                                                 string requestHash,
                                                 CancellationToken cancellationToken = default);

        Task<string> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO,
                                     string key,
                                     string requestHash,
                                     CancellationToken cancellationToken = default);
    }
}
