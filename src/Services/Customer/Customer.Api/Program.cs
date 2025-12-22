
using Customer.Api.Consumers;
using Customer.Api.DevOnly;
using Customer.Application;
using Customer.Application.Abstractions.Persistence;
using Customer.Infrastructure;
using MassTransit;
using SharedKernel.Web.Api;

namespace Customer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddTransient<ExceptionHandlingMiddleware>();
            builder.Services.AddCustomerApplication();
            builder.Services.AddCustomerInfrastructure(builder.Configuration);
            builder.Services.AddSingleton<InMemoryCustomerRepository>();

            builder.Services.AddSingleton<ICustomerRepository>(sp => sp.GetRequiredService<InMemoryCustomerRepository>());
            builder.Services.AddSingleton<ICustomerReadOnlyRepository>(sp => sp.GetRequiredService<InMemoryCustomerRepository>());
            builder.Services.AddSingleton<IUnitOfWork>(sp => sp.GetRequiredService<InMemoryCustomerRepository>());


            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<CreateCustomerCommandConsumer>();

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["AzureServiceBus:ConnectionString"]);

                    // For commands: queue endpoints per consumer
                    cfg.ConfigureEndpoints(context);
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
public partial class Program { }
