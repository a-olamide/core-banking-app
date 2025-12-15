using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.IntegrationTests
{
    public static class DatabaseFixture
    {
        public static async Task EnsureDatabaseMigratedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
            await db.Database.MigrateAsync();
        }

        public static async Task ResetAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            // simple reset for now (OK for demo). Later we can use Respawn.
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }
    }
}
