using APIUsingJWT.Data.Repositories;
using APIUsingJWT.Domain.Repositories;
using APIUsingJWT.Domain.Services;
using APIUsingJWT.Services.RepServices;

namespace APIUsingJWT.StartUpDi
{
    public static class AppRepositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            return services;

        }
    }
}
