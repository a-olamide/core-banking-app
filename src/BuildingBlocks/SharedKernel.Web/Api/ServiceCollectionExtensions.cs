using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedWebKernel(this IServiceCollection services)
        {
            services.AddTransient<ExceptionHandlingMiddleware>();
            return services;
        }
    }
}
