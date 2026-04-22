using Mini_Banking.Domain.Exceptions;

namespace Mini_Banking.Domain.ValueObjects
{
    public class Amount
    {
        const decimal MinValue = (decimal)0.01;
        public decimal Value { get; private set; }

        public Amount(decimal value)
        {
            if (value < MinValue)
                throw new DomainException(DomainErrorCode.InvalidAmount, "amount value can not be zero or negative");

            this.Value = value;
        }
    }
}
