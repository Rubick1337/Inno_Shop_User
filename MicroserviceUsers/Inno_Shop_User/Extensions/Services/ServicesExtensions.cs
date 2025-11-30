using Application.Interfaces;
using Application.Services;

namespace Inno_Shop_User.Extensions.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICodeGenerator, CodeGenerator>();
            return services;
        }
    }
}
