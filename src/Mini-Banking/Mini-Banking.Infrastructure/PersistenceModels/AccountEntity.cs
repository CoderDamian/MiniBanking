namespace Mini_Banking.Infrastructure.PersistenceModels
{
    public class AccountEntity()
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public int Tipo { get; set; }

        public int Currency { get; set; }

        public int OwnerID { get; set; }

        public decimal Balance { get; set; }
    }
}
