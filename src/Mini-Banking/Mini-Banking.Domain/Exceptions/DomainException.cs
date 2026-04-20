namespace Mini_Banking.Domain.Exceptions
{
    internal class DomainException : Exception
    {
        public string Code { get; private set; }
        public string? Details { get; private set; }

        public DomainException(string code,
                               string message) :base(message)
        {
            this.Code = code;
        }

        public DomainException(string code,
                               string message, 
                               string details) : base(message)
        {
            this.Code = code;
            this.Details = details;
        }
    }
}
