using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Cheetah.WebApi.Core.Config;
using Newtonsoft.Json;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class ServiceInstaller
    {
        public static void InstallServices(this IServiceCollection services, IHostEnvironment hostEnvironment, ConfigurationManager configuration)
        {
            //Options
            services.Configure<KafkaProducerConfig>(configuration.GetSection(KafkaProducerConfig.Position));
            services.Configure<KafkaConsumerConfig>(configuration.GetSection(KafkaConsumerConfig.Position));
            services.Configure<PrometheusConfig>(configuration.GetSection(PrometheusConfig.Position));

            //Services
            services.AddHttpContextAccessor();
            // services.AddCheetahOpenSearch(configuration, cfg =>
            // {
            //     cfg.DisableDirectStreaming = hostEnvironment.IsDevelopment();
            //     cfg.WithJsonSerializerSettings(settings =>
            //     {
            //         settings.MissingMemberHandling = MissingMemberHandling.Error;
            //         settings.Converters.Add(new UtcDateTimeConverter());
            //     });
            // });
            // Hosted services
            // services.AddHostedService<TimedOpenSearchTokenRefresherService>();

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