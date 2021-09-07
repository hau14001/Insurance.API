using Be.Infrastructure.AspNetCore;
using Be.Infrastructure.ConfigureAuth;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.ConfigureAuth;
using Insurance.Infrastructure.Data;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Insurance.Infrastructure.AspNetCore;
using Microsoft.IdentityModel.Tokens;

namespace Be.Infrastructure
{
    public static class ConfigureExtensions
    {
        public static void AddBaseConfigure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<Role>>();

            services.AddCors();

            services.AddControllers(options =>
                {
                    options.Filters.Add<HttpStatusCodeFilter>();
                    options.Filters.Add<ModelStateValidatorFilter>();
                })
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

            ConfigureAuth(services, configuration);
            services.AddRepository<ApplicationDbContext, CurrentUser>();
        }

        private static void ConfigureAuth(IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions));
            services.Configure<JwtOptions>(jwtOptions);

            var secret = jwtOptions.Get<JwtOptions>().Secret;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                var permissions = new List<string>();
                permissions.AddRange(new PermissionProvider().GetAll());
                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });
        }

        public static IServiceCollection AddRepository<TDbContext, TCurrentUser>(this IServiceCollection services)
            where TDbContext : DbContext
            where TCurrentUser : ICurrentUser
        {
            services.AddScoped(typeof(ICurrentUser), typeof(TCurrentUser));
            services.AddScoped<IRepository, Repository<TDbContext>>();

            return services;
        }
    }
}