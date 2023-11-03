using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Cheetah.WebApi.Core.Config;
using Cheetah.WebApi.Shared.Infrastructure.ServiceProvider;
using Confluent.Kafka;
using Cheetah.Core.Config;
using Microsoft.Extensions.Logging;
using Cheetah.Core.Infrastructure.Services.Kafka;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    [InstallerPriority(Priorities.AfterConfig)]
    public class KafkaConsumerInstaller : IServiceCollectionInstaller
    {
        public void Install(IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            services.AddSingleton(localProvider =>
            {
                var kafkaConsumerOptions = localProvider.GetRequiredService<IOptions<KafkaConsumerConfig>>();
                var kafkaConfig = localProvider.GetRequiredService<IOptions<KafkaConfig>>();
                var logger = localProvider.GetRequiredService<ILogger<Program>>();
                var applicationLifetime = localProvider.GetRequiredService<IHostApplicationLifetime>();
                var clientConfig = new ClientConfig
                {
                    BootstrapServers = kafkaConfig.Value.Url,
                    SaslMechanism = SaslMechanism.OAuthBearer,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                };

                var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig(clientConfig)
                {
                    GroupId = kafkaConsumerOptions.Value.ConsumerName,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    EnableAutoCommit = true,
                })
                    .SetErrorHandler((consumer1, error) =>
                    {
                        if (error.IsError)
                        {
                            logger.LogError("Kafka fatal error: {Reason}", error.Reason);
                            applicationLifetime.StopApplication();
                            throw new KafkaException(error);
                        }

                        logger.LogWarning("Kafka error: {Reason}", error.Reason);
                    })
                    .AddCheetahOAuthentication(localProvider)
                    .Build();


                return consumer;
            });
        }
    }
}