using Data.Repositories;
using Domain.Interfaces;

namespace Inno_Shop_Product.Extasions.Repostiories
{
    public static class RepositoriesExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
