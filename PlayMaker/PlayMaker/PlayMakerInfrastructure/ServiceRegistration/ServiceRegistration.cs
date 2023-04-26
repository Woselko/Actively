using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerInfrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlayMakerDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("PlayMakerDb")));

            services.AddScoped<PlayMakerDbSeeder>();
        }
    }
}
