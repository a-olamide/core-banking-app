using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.IntegrationTests
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Ensure it can find appsettings.Testing.json in the test project's output
                config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false);

                // Useful later (CI secrets, local overrides)
                config.AddEnvironmentVariables();
            });
        }
    }
}
