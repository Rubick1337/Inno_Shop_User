namespace Inno_Shop_User.Extensions.Configure
{
    public static class HttpExtenson
    {
        public static IServiceCollection AddHttp(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient("Products", client =>
            {
                var baseUrl = configuration["Services:ProductsBaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
            return services;
        }
    }
}
