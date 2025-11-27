using System.Text;
using Application.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Inno_Shop_Product.Extasions.Configure
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<JwtOptions>(
                configuration.GetSection("Jwt"));

            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()!;
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            return services;
        }
    }
}
