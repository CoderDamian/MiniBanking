using Mini_Banking.Application.DTOs;

namespace Mini_Banking.Application.Contracts
{
    public interface IAccountService
    {
        Task<GetAccountByDTOResponse> GetAccountByAsync(int ID, CancellationToken cancellationToken = default);
    }
}
