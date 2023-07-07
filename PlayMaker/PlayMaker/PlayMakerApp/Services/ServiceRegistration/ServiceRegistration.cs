using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Localization;
using PlayMakerInfrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlayMakerApp.Models.Authentication.Email;
using PlayMakerApp.Services.UserServices.EmailService;

namespace PlayMakerApp.Services.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //EmailConfig
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

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

            if (builder.Environment.IsDevelopment()) { mvcBuilder.AddRazorRuntimeCompilation(); }

            services.AddScoped<PlayMakerDbSeeder>();
            services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<PlayMakerDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }
    }
}
