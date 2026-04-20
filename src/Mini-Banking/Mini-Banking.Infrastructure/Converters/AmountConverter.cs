using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Infrastructure.Converters
{
    internal class AmountConverter : ValueConverter<Amount, decimal>
    {
        public AmountConverter() : base(
            amount => amount.Value,     // Amount → DB
            value => new Amount(value)  // DB → Amount
            )
        {

        }
    }
}
