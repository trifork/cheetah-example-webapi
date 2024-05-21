using Cheetah.WebApi.Core.Config;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class FluentValidationInstaller
    {
        public static void InstallFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidation(config =>
            {
                config.AutomaticValidationEnabled = false;
                config.RegisterValidatorsFromAssemblyContaining<AssemblyAnchor>();
            });
        }
    }
}
