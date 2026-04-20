using Mini_Banking.Application.Contracts;

namespace Mini_Banking.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly MyDBContext _myDBContext;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        private readonly IBankTransactionRepository _bankTransactionRepository;

        public UnitOfWork(MyDBContext myDBContext, IUserRepository userRepository, IAccountRepository accountRepository, IBankTransactionRepository bankTransactionRepository)
        {
            this._myDBContext = myDBContext;
            this._userRepository = userRepository;
            this._accountRepository = accountRepository;
            this._bankTransactionRepository = bankTransactionRepository;
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
            return await _myDBContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
