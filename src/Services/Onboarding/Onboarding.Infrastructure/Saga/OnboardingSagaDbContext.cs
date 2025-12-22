using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Infrastructure.Saga
{
    public sealed class OnboardingSagaDbContext : SagaDbContext
    {
        public OnboardingSagaDbContext(DbContextOptions<OnboardingSagaDbContext> options) : base(options) { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OnboardingStateMap(); }
        }
    }
}
