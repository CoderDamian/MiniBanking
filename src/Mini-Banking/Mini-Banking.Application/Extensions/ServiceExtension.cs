using Microsoft.Extensions.DependencyInjection;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.Sevices;

namespace Mini_Banking.Application.Extensions
{
    internal static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankTransactionService, BankTransactionService>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }
}
