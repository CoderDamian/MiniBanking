namespace Mini_Banking.Application.Exceptions
{
    public static class ApplicationErrorCode
    {
        public const string IdempotencyInvalid = "idempotency.idempotency_conflict";
        public const string IdempotencyConflict = "idempotency.idempotency_conflict";
    }
}
