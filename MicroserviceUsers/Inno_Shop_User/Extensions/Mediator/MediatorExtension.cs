using Application.Users.Queries.GetAllUsers;

namespace Inno_Shop_User.Extensions.Mediator
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediator (this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetAllUsersQuery).Assembly);
            });
            return services;
        }
      
    }
 }
