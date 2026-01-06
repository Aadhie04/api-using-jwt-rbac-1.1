using APIUsingJWT.Domain.Services;
using APIUsingJWT.Services.RepServices;
using Microsoft.AspNetCore.Identity;

namespace APIUsingJWT.StartUpDi
{
    public static class AppServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            return services;
        }
    }
}
