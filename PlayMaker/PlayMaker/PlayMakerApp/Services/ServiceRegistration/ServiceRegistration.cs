using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Localization;
using PlayMakerInfrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PlayMakerApp.Services.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddLocalization(options => options.ResourcesPath = "PlayMaker\\Resources");
            services.AddDbContext<PlayMakerDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("WoselkoConnectionStringDev_PlayMakerDb_v1")));

            var mvcBuilder = services.AddControllersWithViews().AddDataAnnotationsLocalization(options =>
            {
                var type = typeof(Resources.Common);
                var assembly = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                var localizer = factory.Create("Common", assembly.Name);
                options.DataAnnotationLocalizerProvider = (t, f) => localizer;
            });

            //if (builder.Environment.IsDevelopment()) { mvcBuilder.AddRazorRuntimeCompilation(); }

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<PlayMakerDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddScoped<PlayMakerDbSeeder>();
        }
    }
}
