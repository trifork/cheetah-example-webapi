using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Cheetah.WebApi.Core.Config;
using Cheetah.WebApi.Shared.Infrastructure.Auth;
using Cheetah.WebApi.Shared.Config;
using Cheetah.Core.Interfaces;
using Cheetah.Core.Infrastructure.Services.OpenSearchClient;
using Cheetah.WebApi.Shared.Infrastructure.ServiceProvider;
using Cheetah.Core.Config;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    [InstallerPriority(Priorities.Default)]
    public class ServiceInstaller : IServiceCollectionInstaller
    {
        public void Install(IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            //Options
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.Configure<OAuthConfig>(configuration.GetSection(OAuthConfig.Position));
            services.Configure<KafkaConfig>(configuration.GetSection(KafkaConfig.Position));
            services.Configure<OpenSearchConfig>(configuration.GetSection(OpenSearchConfig.Position));
            services.Configure<KafkaProducerConfig>(configuration.GetSection(KafkaProducerConfig.Position));
            services.Configure<KafkaConsumerConfig>(configuration.GetSection(KafkaConsumerConfig.Position));
            services.Configure<PrometheusConfig>(configuration.GetSection(PrometheusConfig.Position));

            //Services
            services.AddHttpContextAccessor();
            services.AddTransient<ICheetahOpenSearchClient, CheetahOpenSearchClient>();

            // Hosted services
            services.AddHostedService<TimedOpenSearchTokenRefresherService>();

            //Cache
            services.AddMemoryCache();

            //Httpclient
            var applicationName = !string.IsNullOrEmpty(hostEnvironment.ApplicationName) ? hostEnvironment.ApplicationName : Dns.GetHostName();

            services.AddHttpClient("",
                    (provider, client) => { client.DefaultRequestHeaders.Add("User-Agent", applicationName); })
                .UseHttpClientMetrics();
        }
    }
}