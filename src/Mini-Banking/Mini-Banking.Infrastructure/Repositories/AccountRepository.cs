using Microsoft.EntityFrameworkCore;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Domain.Entities;
using Mini_Banking.Domain.Enums;

namespace Mini_Banking.Infrastructure.Repositories
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly MyDBContext _myDBContext;

        public AccountRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Account?> GetByAsync(int id, CancellationToken cancellationToken = default)
        {
            var accountEntity = await _myDBContext.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (accountEntity is null)
                return null;

            var account = new Account(id: accountEntity.Id,
                                      numero: accountEntity.Numero,
                                      tipo: (AccountType)accountEntity.Tipo,
                                      currency: (CurrencyType)accountEntity.Currency,
                                      ownerID: accountEntity.OwnerID,
                                      balance: accountEntity.Balance);

            return account;
        }

        public async Task UpdateBalanceAsync(Account account, CancellationToken cancellationToken = default)
        {
            var accountEntity = await _myDBContext.Accounts
                .FindAsync(account.ID)
                .ConfigureAwait(false);

            if (accountEntity is null)
                throw new Exception();

            accountEntity.Balance = account.Balance;
        }
    }
}
