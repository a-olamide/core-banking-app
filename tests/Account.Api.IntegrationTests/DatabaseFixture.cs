using Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Api.IntegrationTests
{
    public static class DatabaseFixture
    {
        public static async Task ResetAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }
    }
}
