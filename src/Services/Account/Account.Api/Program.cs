
using Account.Application;
using Account.Infrastructure;
using Microsoft.OpenApi.Models;
using SharedKernel.Web.Api;
using System.Text.Json.Serialization;

namespace Account.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Account API", Version = "v1" });
            });

            builder.Services.AddAccountApplication();
            builder.Services.AddAccountInfrastructure(builder.Configuration);

            // Shared middleware
            builder.Services.AddTransient<ExceptionHandlingMiddleware>();


            var app = builder.Build();
           
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
