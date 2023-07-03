using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlayMakerInfrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlayMakerDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("BulczoConnectionStringDev_PlayMakerDb")));

            services.AddScoped<PlayMakerDbSeeder>();
        }
    }
}
