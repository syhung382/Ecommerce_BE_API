using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce_BE_API.WebApi.BackgroundServices
{
    public static class ConfigureScheduleService
    {
        public static IServiceCollection ConfigureBackground(this IServiceCollection services)
        {
           
            //services.AddHostedService<SharedRevenueScheduleService>();
            return services;
        }
    }
}
