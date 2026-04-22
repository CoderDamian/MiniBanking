using Microsoft.EntityFrameworkCore;
using Mini_Banking.Infrastructure.PersistenceModels;
using Mini_Banking.Infrastructure.Mappers;

namespace Mini_Banking.Infrastructure
{
    internal class MyDBContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
        public DbSet<TransactionEntity> BankTransactions => Set<TransactionEntity>();
        public DbSet<IdempotencyEntity> Idempotency => Set<IdempotencyEntity>();


        public MyDBContext(DbContextOptions<MyDBContext> opt) : base(opt)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserMap).Assembly);
        }
    }
}
