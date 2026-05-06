using Microsoft.EntityFrameworkCore;
using Mini_Banking.Application.Contracts;

namespace Mini_Banking.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly MyDBContext _myDBContext;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        private readonly IBankTransactionRepository _bankTransactionRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public UnitOfWork(MyDBContext myDBContext, IUserRepository userRepository, IAccountRepository accountRepository, IBankTransactionRepository bankTransactionRepository, IIdempotencyRepository idempotencyRepository)
        {
            this._myDBContext = myDBContext;
            this._userRepository = userRepository;
            this._accountRepository = accountRepository;
            this._bankTransactionRepository = bankTransactionRepository;
            this._idempotencyRepository = idempotencyRepository;
        }

        public IUserRepository UserRepository
        {
            get => _userRepository;
        }

        public IAccountRepository AccountRepository
        {
            get => _accountRepository;
        }
        
        public IBankTransactionRepository BankTransactionRepository
        {
            get => _bankTransactionRepository;
        }

        public IIdempotencyRepository IdempotencyRepository
        {
            get => _idempotencyRepository;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext.Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext.Database.CommitTransactionAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _myDBContext.Dispose();
        }

        public async Task DisposeAsync()
        {
            await _myDBContext.DisposeAsync().ConfigureAwait(false);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext.Database.RollbackTransactionAsync().ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _myDBContext
                        .SaveChangesAsync()
                        .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The record was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
