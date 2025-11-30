using Data.Repositories;
using Domain.Interfaces;

namespace Inno_Shop_User.Extensions.Repositories
{
    public static class RepositoriesExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRedisRepository, RedisRepository>();

            return services;
        }
    }
}
