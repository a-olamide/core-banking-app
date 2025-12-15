using Customer.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Persistence.Configurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<CustomerAggregate>
    {
        public void Configure(EntityTypeBuilder<CustomerAggregate> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Owned: PersonName
            builder.OwnsOne(x => x.Name, nb =>
            {
                nb.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
                nb.Property(p => p.MiddleName).HasMaxLength(100);
                nb.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            });

            // Owned: Email
            builder.OwnsOne(x => x.Email, eb =>
            {
                eb.Property(p => p.Value).HasColumnName("Email").HasMaxLength(320).IsRequired();
                eb.HasIndex(p => p.Value).IsUnique(); // Unique constraint

                //useful to return duplicate email when you try create customer with existing emial
                eb.HasIndex(p => p.Value)
                  .IsUnique()
                  .HasDatabaseName("UX_Customers_Email");
            });

            // Owned: PhoneNumber
            builder.OwnsOne(x => x.PhoneNumber, pb =>
            {
                pb.Property(p => p.CountryCode).HasMaxLength(6).IsRequired();
                pb.Property(p => p.Number).HasMaxLength(20).IsRequired();
            });

            // Owned: Address
            builder.OwnsOne(x => x.Address, ab =>
            {
                ab.Property(p => p.Line1).HasMaxLength(200).IsRequired();
                ab.Property(p => p.Line2).HasMaxLength(200);
                ab.Property(p => p.City).HasMaxLength(100).IsRequired();
                ab.Property(p => p.StateOrProvince).HasMaxLength(100).IsRequired();
                ab.Property(p => p.PostalCode).HasMaxLength(20).IsRequired();
                ab.Property(p => p.CountryCode).HasMaxLength(2).IsRequired();
            });

            // Ignore domain events (not persisted)
            builder.Ignore(x => x.DomainEvents);
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();

        }
    }
}
