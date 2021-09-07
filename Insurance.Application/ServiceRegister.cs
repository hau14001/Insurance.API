using Insurance.Application.Common.Auth;
using Insurance.Application.Common.Permissions;
using Insurance.Application.Common.Roles;
using Insurance.Application.Common.Users;
using Insurance.Application.UploadFiles;
using Insurance.Domain.Entities.BaseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Application
{
    public static class ServiceRegister
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IAuthService, AuthService<User>>();
            services.AddScoped<IUserService, UserService<User, Role, UserRole>>();
            services.AddScoped<IRoleService, RoleService<User, Role, UserRole>>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IPermissionChecker, PermissionChecker<User, UserRole>>();

            //custom

            services.AddScoped<IUploadFileService, UploadFileService>();

            return services;
        }
    }
}