using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop_Product.Extasions.Configure
{
    public static class ConfigureExtansion
    {
        public static IServiceCollection AddConfigure(this IServiceCollection services,
    IConfiguration configuration)
        {
            services.AddDbContext<ProductContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
