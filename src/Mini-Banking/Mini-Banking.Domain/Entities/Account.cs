using Mini_Banking.Domain.Enums;
using Mini_Banking.Domain.Exceptions;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Domain.Entities
{
    internal class Account : Entity
    {
        public string Numero { get; private set; }
        public AccountType Tipo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public string UserDNI { get; private set; }
        public decimal Balance { get; private set; }

        public Account(string numero, AccountType tipo, CurrencyType currency, string userDNI)
        {
            this.Numero = numero;
            this.Tipo = tipo;
            this.Currency = currency;
            this.UserDNI = userDNI;
        }

        public decimal GetBalance =>
            this.Balance;

        public void Credit(Amount amount)
        {
            this.Balance += amount.Value;
        }

        public void Debit(Amount amount)
        {
            if (!HasSufficientFunds(amount.Value))
                throw new DomainException(DomainErrorCodes.InsufficientBalance, "insufficience balance to the operation");

            this.Balance -= amount.Value;
        }

        public bool HasSufficientFunds(decimal amount) =>
            this.Balance >= amount;
    }
}
