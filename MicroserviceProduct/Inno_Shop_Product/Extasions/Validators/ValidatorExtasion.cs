using FluentValidation;
using Inno_Shop_Product.Validators.ProductValidation;

namespace Inno_Shop_Product.Extasions.Validators
{
    public static class ValidatorExtasion
    {
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(new[]
{
                typeof(CreateProductValidator).Assembly,
                typeof(UpdateProductValidator).Assembly,
            });
            return services;
        }
    }
}
