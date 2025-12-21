using Account.Application.Abstractions.Persistence;
using Account.Application.Abstractions.Services;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Repositories;
using Account.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAccountInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("AccountDb"));
            });

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountReadOnlyRepository, AccountReadOnlyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IAccountNumberGenerator, AccountNumberGenerator>();

            return services;
        }
    }
}
