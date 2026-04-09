using Mini_Banking.Application.Contracts;
using Mini_Banking.Domain.Entities;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Application.Sevices
{
    internal class BankTransaction : IBankTransaction
    {
        public int Deposit(Account receiver, decimal amount)
        {
            var amountToDeposit = new Amount(amount);
            
            var deposit = Transaction.FromDeposit(receiver, amountToDeposit);

            deposit.Execute();

            return deposit.ID;
        }

        public int Transfer(Account sender, Account receiver, decimal amount)
        {
            var amountToTransfer = new Amount(amount);

            var transfer = Transaction.FromTransfer(sender, receiver, amountToTransfer);

            transfer.Execute();

            return transfer.ID;
        }

        public int Withdrawal(Account sender, decimal amount)
        {
            var amountToWithdrawal = new Amount(amount);

            var withdrawal = Transaction.FromWithdrawal(sender, amountToWithdrawal);

            withdrawal.Execute();

            return withdrawal.ID;
        }
    }
}
