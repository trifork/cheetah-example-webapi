using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Cheetah.WebApi.Shared.Infrastructure.ServiceProvider;
using Cheetah.WebApi.Core.Config;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    [InstallerPriority(Priorities.AfterConfig)]
    public class HealthCheckInstaller : IServiceCollectionInstaller
    {
        public void Install(IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            services.AddHealthChecks()
                .ForwardToPrometheus();
        }
    }
}

