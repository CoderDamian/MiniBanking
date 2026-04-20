using Mini_Banking.Domain.Entities;

namespace Mini_Banking.Application.Contracts
{
    public interface IBankTransactionRepository
    {
        Task<Guid> AddAsync(BankTransaction transaction, CancellationToken cancellationToken = default);
        Task MarkAsCompletedAsync(Guid transactionID, CancellationToken cancellationToken = default);
    }
}
