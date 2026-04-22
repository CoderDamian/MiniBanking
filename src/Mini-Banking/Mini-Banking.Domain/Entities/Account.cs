using Mini_Banking.Domain.Enums;
using Mini_Banking.Domain.Exceptions;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Domain.Entities
{
    public class Account : Entity
    {
        public string Numero { get; private set; } = string.Empty;
        public AccountType Tipo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Balance { get; private set; }
        public int OwnerID { get; private set; }

        private Account() : base(0)
        {

        }

        public Account(int id, string numero, AccountType tipo, CurrencyType currency, int ownerID, decimal balance) : base(id)
        {
            this.Numero = numero;
            this.Tipo = tipo;
            this.Currency = currency;
            this.OwnerID = ownerID;
            this.Balance = balance;
        }

        public void Credit(Amount amount)
        {
            this.Balance += amount.Value;
        }

        public void Debit(Amount amount)
        {
            if (!HasSufficientFunds(amount.Value))
                throw new DomainException(DomainErrorCode.InsufficientBalance, "insufficience balance to the operation");

            this.Balance -= amount.Value;
        }

        public bool HasSufficientFunds(decimal amount) =>
            this.Balance >= amount;
    }
}
