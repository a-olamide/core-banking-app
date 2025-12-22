using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onboarding.Infrastructure.Saga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOnboardingInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<OnboardingSagaDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("OnboardingDb")));

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddSagaStateMachine<OnboardingStateMachine, OnboardingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.AddDbContext<DbContext, OnboardingSagaDbContext>((provider, cfg) =>
                        {
                            cfg.UseSqlServer(config.GetConnectionString("OnboardingDb"));
                        });
                    });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(config["AzureServiceBus:ConnectionString"]);
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
