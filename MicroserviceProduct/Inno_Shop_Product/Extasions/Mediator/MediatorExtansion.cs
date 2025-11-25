using Application.Products.Commands.CreateProduct;

namespace Inno_Shop_Product.Extasions.Mediator
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
            });
            return services;
        }
    }
}
