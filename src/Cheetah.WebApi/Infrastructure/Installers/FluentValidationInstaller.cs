using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cheetah.WebApi.Shared.Infrastructure.ServiceProvider;
using Cheetah.WebApi.Core.Config;

namespace Cheetah.WebApi.Infrastructure.Installers
{

    [InstallerPriority(Priorities.BeforeConfig)]
    public class FluentValidationInstaller : IServiceCollectionInstaller
    {
        public void Install(IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            services.AddFluentValidation(config =>
            {
                config.AutomaticValidationEnabled = false;
                config.RegisterValidatorsFromAssemblyContaining<AssemblyAnchor>();
            });

        }
    }
}

