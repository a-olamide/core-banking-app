using Account.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure.Persistence.Configurations
{
    public sealed class HoldConfiguration : IEntityTypeConfiguration<Hold>
    {
        public void Configure(EntityTypeBuilder<Hold> builder)
        {
            builder.ToTable("AccountHolds");

            builder.HasKey(x => x.Id);

            builder.Property<Guid>("AccountId").IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.PlacedAt).IsRequired();
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.ReleasedAt);

            builder.Property(x => x.ReleaseReason)
                .HasMaxLength(200);

            // Owned: Amount (Money)
            builder.OwnsOne(x => x.Amount, mb =>
            {
                mb.Property(p => p.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
                mb.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });

            builder.HasIndex("AccountId").HasDatabaseName("IX_AccountHolds_AccountId");
            builder.HasIndex(x => x.Status).HasDatabaseName("IX_AccountHolds_Status");
        }
    }
}
