using Mini_Banking.Domain.Enums;
using Mini_Banking.Domain.Exceptions;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Domain.Entities
{
    public class BankTransaction : Entity
    {
        public int? SenderID { get; private set; }
        public Account? SenderAccount { get; private set; }
        public int? ReceiverID { get; private set; }
        public Account? ReceiverAccount { get; private set; }
        public Amount Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransactionType Type { get; private set; }
        public BankTransactionStatus Status { get; private set; }

        private BankTransaction()
        {
            
        }

        private BankTransaction(Account? senderAccount,
                            Account? receiverAccount,
                            Amount amount,
                            TransactionType type,
                            BankTransactionStatus status)
        {
            this.SenderID = senderAccount is null ? null : senderAccount.ID;
            this.SenderAccount = senderAccount;
            this.ReceiverID = receiverAccount is null ? null : receiverAccount.ID;
            this.ReceiverAccount = receiverAccount;
            this.Amount = amount;
            this.CreatedAt = DateTime.UtcNow;
            this.Type = type;
            this.Status = status;
        }

        //public Transaction(Guid id, int? senderID, int? receiverID, Amount amount, TransactionType type, TransactionStatus status)
        //{
        //    this.ID = id;
        //    this.SenderID = senderID;
        //    this.ReceiverID = receiverID;
        //    this.Amount = amount;
        //    this.CreatedAt = DateTime.UtcNow;
        //    this.Type = type;
        //    this.Status = status;
        //}

        public static BankTransaction FromDeposit(Account receiver, Amount amount) =>
            new(null,
                 receiver,
                 amount,
                 TransactionType.Deposit,
                 BankTransactionStatus.Pending);

        public static BankTransaction FromWithdrawal(Account sender, Amount amount) =>
            new(sender,
                 null,
                 amount,
                 TransactionType.Withdrawal,
                 BankTransactionStatus.Pending);

        public static BankTransaction FromTransfer(Account sender, Account receiver, Amount amount) =>
            new(sender,
                 receiver,
                 amount,
                 TransactionType.Transfer,
                 BankTransactionStatus.Pending);

        public void MarkSuccessTransaction() =>
            this.Status = BankTransactionStatus.Success;

        public void Execute()
        {
            try
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
            catch (Exception)
            {
                MarkFailedTransaction();
                throw;
            }
        }

        private void ExecuteTransfer()
        {
            if (SenderAccount is null || ReceiverAccount is null)
                throw new DomainException(DomainErrorCode.AccountIsNull, "the sender and receiver can not be null");

            SenderAccount.Debit(Amount);
            ReceiverAccount.Credit(Amount);
        }

        private void ExecuteWithdrawal()
        {
            if (SenderAccount is null)
                throw new DomainException(DomainErrorCode.AccountIsNull, "the sender can not be null");

            SenderAccount.Debit(Amount);
        }

        private void ExecuteDeposit()
        {
            if (ReceiverAccount is null)
                throw new DomainException(DomainErrorCode.AccountIsNull, "the receiver can not be null");

            ReceiverAccount.Credit(Amount);
        }

        private void MarkFailedTransaction() =>
            this.Status = BankTransactionStatus.Failed;
    }
}
