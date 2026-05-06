namespace Mini_Banking.Infrastructure.PersistenceModels
{
    internal class TransactionEntity() : EntityPersistenceModel
    {
        public Guid? ID { get; set; }
        public int? SenderFK { get; set; }
        public int? ReceiverFK { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TransactionType { get; set; }
        public int TransactionStatus { get; set; }
    }
}
