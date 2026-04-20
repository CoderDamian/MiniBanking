namespace Mini_Banking.Domain.Exceptions
{
    internal static class DomainErrorCodes
    {
        public const string AccountIsNull = "account.account_not_be_null";
        public const string EntityInvalidData = "entity.invalid_data";
        public const string InvalidAmount = "amount.negative_value";
        public const string InsufficientBalance = "balance.insufficient_balance";
    }
}
