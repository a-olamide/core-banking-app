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
    public sealed class AccountConfiguration : IEntityTypeConfiguration<AccountAggregate>
    {
        public void Configure(EntityTypeBuilder<AccountAggregate> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.AccountType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Owned: AccountNumber
            builder.OwnsOne(x => x.AccountNumber, nb =>
            {
                nb.Property(p => p.Value)
                  .HasColumnName("AccountNumber")
                  .HasMaxLength(10)
                  .IsRequired();

                // Unique account number
                nb.HasIndex(p => p.Value)
                  .IsUnique()
                  .HasDatabaseName("UX_Accounts_AccountNumber");
            });

            // Owned: BookBalance (Money)
            builder.OwnsOne(x => x.BookBalance, mb =>
            {
                mb.Property(p => p.Amount).HasColumnName("BookBalanceAmount").HasPrecision(18, 2).IsRequired();
                mb.Property(p => p.Currency).HasColumnName("BookBalanceCurrency").HasMaxLength(3).IsRequired();
            });

            // Owned: AvailableBalance (Money)
            builder.OwnsOne(x => x.AvailableBalance, mb =>
            {
                mb.Property(p => p.Amount).HasColumnName("AvailableBalanceAmount").HasPrecision(18, 2).IsRequired();
                mb.Property(p => p.Currency).HasColumnName("AvailableBalanceCurrency").HasMaxLength(3).IsRequired();
            });

            // Holds & Liens as separate tables
            builder.Navigation(x => x.Holds).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(x => x.Liens).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Holds)
                .WithOne()
                .HasForeignKey("AccountId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Liens)
                .WithOne()
                .HasForeignKey("AccountId")
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);

            // Helpful index for lookups
            builder.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Accounts_CustomerId");
        }
    }
}
