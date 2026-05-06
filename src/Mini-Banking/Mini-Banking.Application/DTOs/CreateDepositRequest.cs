namespace Mini_Banking.Application.DTOs
{
    public record CreateDepositRequest(int ReceiverAccountID, decimal Amount)
    {
    }
}
