using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Infrastructure.Saga
{
    public sealed class OnboardingStateMap : SagaClassMap<OnboardingState>
    {
        protected override void Configure(EntityTypeBuilder<OnboardingState> entity, ModelBuilder model)
        {
            entity.ToTable("OnboardingSaga");

            entity.HasKey(x => x.CorrelationId);

            entity.Property(x => x.CurrentState)
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(x => x.AccountNumber)
                .HasMaxLength(20);

            entity.Property(x => x.ErrorCode)
                .HasMaxLength(64);

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(512);

            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasIndex(x => x.CreatedAt);
        }
    }
}
