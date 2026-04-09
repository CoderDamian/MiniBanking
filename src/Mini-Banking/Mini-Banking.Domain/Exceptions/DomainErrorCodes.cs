using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_Banking.Domain.Exceptions
{
    internal static class DomainErrorCodes
    {
        public const string InvalidAmount = "amount.negative_value";
        public const string InsufficientBalance = "balance.insufficient_balance";
    }
}
