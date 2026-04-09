using Mini_Banking.Domain.Exceptions;

namespace Mini_Banking.Domain.ValueObjects
{
    internal class Amount
    {
        public decimal Value { get; private set; }

        public Amount(decimal value)
        {
            if (value < 1)
                throw new DomainException(DomainErrorCodes.InvalidAmount, "amount value can not be zero or negative");

            this.Value = value;
        }
    }
}
