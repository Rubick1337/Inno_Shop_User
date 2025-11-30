using Week_3_Inno_PreTrainee.Data.Context;
using Microsoft.EntityFrameworkCore;
using Application.Configuration;

namespace Inno_Shop_User.Extensions.Configure
{
    public static class ConfigureExtension
    {
        public static IServiceCollection AddConfigure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UserContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Codes_";
            });

            return services;
        }
    }
}
