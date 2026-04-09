using Mini_Banking.Domain.Enums;
using Mini_Banking.Domain.Exceptions;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Domain.Entities
{
    internal class Transaction : Entity
    {
        public Account? SenderAccount { get; private set; }
        public Account? ReceiverAccount { get; private set; }
        public Amount Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        private Transaction(Account? senderAccount,
                            Account? receiverAccount,
                            Amount amount,
                            TransactionType type,
                            TransactionStatus status)
        {
            this.SenderAccount = senderAccount;
            this.ReceiverAccount = receiverAccount;
            this.Amount = amount;
            this.CreatedAt = DateTime.UtcNow;
            this.Type = type;
            this.Status = status;
        }

        public static Transaction FromDeposit(Account receiver, Amount amount) =>
            new (null,
                 receiver,
                 amount,
                 TransactionType.Deposit,
                 TransactionStatus.Pending);

        public static Transaction FromWithdrawal(Account sender, Amount amount) =>
            new (sender,
                 null,
                 amount,
                 TransactionType.Withdrawal,
                 TransactionStatus.Pending);

        public static Transaction FromTransfer(Account sender, Account receiver, Amount amount) =>
            new(sender,
                 receiver,
                 amount,
                 TransactionType.Withdrawal,
                 TransactionStatus.Pending);

        public void MarkSuccessTransaction() =>
            this.Status = TransactionStatus.Success;

        public void Execute()
        {
            switch (this.Type)
            {
                case TransactionType.Deposit:
                    ExecuteDeposit();
                    break;
                case TransactionType.Withdrawal:
                    ExecuteWithdrawal();
                    break;
                case TransactionType.Transfer:
                    ExecuteTransfer();
                    break;
                default:
                    break;
            }
        }

        private void ExecuteTransfer()
        {
            if (SenderAccount is null || ReceiverAccount is null)
                throw new DomainException(DomainErrorCodes.AccountIsNull, "the sender and receiver can not be null");

            SenderAccount.Debit(Amount);
            ReceiverAccount.Credit(Amount);

            MarkSuccessTransaction();
        }

        private void ExecuteWithdrawal()
        {
            if (SenderAccount is null)
                throw new DomainException(DomainErrorCodes.AccountIsNull, "the sender can not be null");

            SenderAccount.Debit(Amount);

            MarkSuccessTransaction();
        }

        private void ExecuteDeposit()
        {
            if (ReceiverAccount is null)
                throw new DomainException(DomainErrorCodes.AccountIsNull, "the receiver can not be null");

            ReceiverAccount.Credit(Amount);

            MarkSuccessTransaction();
        }
    }
}
