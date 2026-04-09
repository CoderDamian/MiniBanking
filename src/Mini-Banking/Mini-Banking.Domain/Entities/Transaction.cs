using Mini_Banking.Domain.Enums;

namespace Mini_Banking.Domain.Entities
{
    internal class Transaction : Entity
    {
        public int SenderAccountID { get; private set; }
        public int ReceiverAccountID { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        private Transaction(int senderAccountID,
                            int receiverAccountID,
                            decimal amount,
                            TransactionType type,
                            TransactionStatus status)
        {
            this.SenderAccountID = senderAccountID;
            this.ReceiverAccountID = receiverAccountID;
            this.Amount = amount;
            this.CreatedAt = DateTime.UtcNow;
            this.Type = type;
            this.Status = status;
        }

        public static Transaction FromDeposit(int receiverAccountID, decimal amount)
        {
            return new Transaction(0,
                                   receiverAccountID,
                                   amount,
                                   TransactionType.Deposit,
                                   TransactionStatus.Pending);
        }
    }
}
