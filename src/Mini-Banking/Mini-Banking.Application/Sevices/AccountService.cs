using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using Mini_Banking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_Banking.Application.Sevices
{
    internal class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<GetAccountByDTOResponse> GetAccountByAsync(int id, CancellationToken cancellationToken = default)
        {
            var account = await _unitOfWork.AccountRepository
                .GetByAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
                throw new ArgumentNullException();

            var user = await _unitOfWork.UserRepository
                .GetUserBy(id, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                throw new ArgumentNullException();

            var response = new GetAccountByDTOResponse(DNI: user.DNI,
                                                       Correo: user.Email,
                                                       AccountID: account.ID,
                                                       NumeroCuenta: account.Numero,
                                                       Balance: account.Balance);

            return response;
        }
    }
}
