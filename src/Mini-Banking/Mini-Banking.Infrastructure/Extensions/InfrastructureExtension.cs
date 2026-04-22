using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Infrastructure.Repositories;

namespace Mini_Banking.Infrastructure.Extensions
{
    internal static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddDbContext<MyDBContext>(opt => opt.UseOracle("user id=dev;password=Barcelona!2026;data source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1521)(host=adb.sa-santiago-1.oraclecloud.com))(connect_data=(service_name=g2cbfc2de6ed2c2_myerp_high.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));"));
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            return services;
        }
    }
}
