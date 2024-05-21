using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class HealthCheckInstaller
    {
        public static void InstallHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks().ForwardToPrometheus();
        }
    }
}
