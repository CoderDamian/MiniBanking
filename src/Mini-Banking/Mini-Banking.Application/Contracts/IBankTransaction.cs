using Mini_Banking.Domain.Entities;

namespace Mini_Banking.Application.Contracts
{
    internal interface IBankTransaction
    {
        int Deposit(Account receiver, decimal amount);
        int Withdrawal(Account sender, decimal amount);
        int Transfer(Account receiver, Account sender, decimal amount);
    }
}
