using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce_BE_API.DbContext.Base;
using Ecommerce_BE_API.DbContext.Common;
using Ecommerce_BE_API.DbContext.Models;

namespace Ecommerce_BE_API.DbContext
{
    public static class DbContextDI
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, string connectString)
        {
            services.AddScoped<IContext, Ecommerce_BE_APIContext>();
            services.AddScoped(typeof(IGenericDbContext<>), typeof(GenericDbContext<>));
            services.AddDbContext<Ecommerce_BE_APIContext>(options => options.UseSqlServer(connectString, o => o.CommandTimeout(180)));
            return services;
        }
    }
}
