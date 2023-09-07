using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Localization;
using ActivelyInfrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ActivelyApp.Services.UserServices.EmailService;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ActivelyApp.Mappings;
using System.Text.Json.Serialization;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using ActivelyApp.Services.EntityService;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using ActivelyApp.Models.AuthenticationDto.Email;
using ActivelyApp.Models.Common;

namespace ActivelyApp.Services.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //Database
            services.AddDbContext<ActivelyDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("BulczoConnectionStringDev_ActivelyDb")));
            services.AddScoped<ActivelyDbSeeder>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            //Business logic services
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IGameService, GameService>();
            //mapper
            services.AddAutoMapper(typeof(Program));
            //Authentication
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ActivelyDbContext>()
                .AddDefaultTokenProviders();
            //json
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            //Required Email confirmation
            builder.Services.Configure<IdentityOptions>(
                opts => opts.SignIn.RequireConfirmedEmail = true);
            //Reset password token life (token expiration)
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));
            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });
            builder.Services.AddAuthorization();

            //Email sender config
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            services.AddScoped<IEmailService, EmailService>();

            //Resources
            services.Configure<LanguageSettings>(options => builder.Configuration.GetSection("LanguageSettings").Bind(options));
            services.AddLocalization(options => options.ResourcesPath = "Actively\\Resources");
            var mvcBuilder = services.AddControllersWithViews().AddDataAnnotationsLocalization(options =>
            {
                var type = typeof(Resources.Common);
                var assembly = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                var localizer = factory.Create("Common", assembly.Name);
                options.DataAnnotationLocalizerProvider = (t, f) => localizer;
            });

            //Other
            if (builder.Environment.IsDevelopment()) { mvcBuilder.AddRazorRuntimeCompilation(); }

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

        }
    }
}
