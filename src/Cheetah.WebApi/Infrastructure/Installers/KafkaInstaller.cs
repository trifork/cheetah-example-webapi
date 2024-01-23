using Microsoft.Extensions.DependencyInjection;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Cheetah.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Cheetah.Kafka.Configuration;
using Cheetah.Kafka;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class KafkaInstaller
    {
        public static void InstallKafka(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var kafkaConsumerOptions = configuration.Get<KafkaConsumerConfig>();
            var kafkaProducersOptions = configuration.Get<KafkaProducerConfig>();

            services.AddCheetahKafka(configuration, options =>
            {
                options.ConfigureDefaultConsumer(config =>
                {
                    config.AllowAutoCreateTopics = false;
                    config.GroupId = kafkaConsumerOptions.ConsumerName;
                    config.AutoOffsetReset = AutoOffsetReset.Earliest;
                    config.EnableAutoCommit = false;
                });
                options.ConfigureDefaultProducer(config =>
                {
                    config.AllowAutoCreateTopics = false;
                    config.ClientId = kafkaProducersOptions.ProducerName;
                    config.EnableBackgroundPoll = true;
                });
            })
            // todo: SetErrorHandler && SecurityProtocol
            .WithConsumer<Ignore, string>(x =>
            {
                x.GroupId = kafkaConsumerOptions.ConsumerName;
            })
            .WithProducer<Null, string>(x =>
            {
                x.ClientId = kafkaProducersOptions.ProducerName;
            });

            // services.AddSingleton<IProducer<Null, string>>((localProvider) =>
            // {
            //     var kafkaConfig = localProvider.GetRequiredService<IOptions<KafkaConfig>>();
            //     var factory = localProvider.GetRequiredService<KafkaClientFactory>();
            //     factory.CreateProducerBuilder<Null, string>(kafkaConfig.Value.Url)
            //         .AddCheetahOAuthentication(localProvider)
            //         .Build();

            //     return new ProducerBuilder<Null, string>(
            //                     new ProducerConfig
            //                     {
            //                         BootstrapServers = kafkaConfig.Value.Url,
            //                         SaslMechanism = SaslMechanism.OAuthBearer,
            //                         SecurityProtocol = SecurityProtocol.SaslPlaintext,
            //                     })
            //                     .AddCheetahOAuthentication(localProvider)
            //                     .Build();
            // });
        }
    }
}