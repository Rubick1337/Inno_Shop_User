using FluentValidation;
using Inno_Shop_User.Validators.UserValidation;
using Inno_Shop_User.Validators.Validation;

namespace Inno_Shop_User.Extensions.Validator
{
    public static class ValidatorExtension
    {
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(new[]
{
                typeof(UserRegistrartionValidator).Assembly,
                typeof(UserUpdateValidator).Assembly,
                typeof(UserLoginValidator).Assembly
            });
            return services;
        }
    }
}
