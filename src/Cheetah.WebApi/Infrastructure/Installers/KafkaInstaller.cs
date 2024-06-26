using Cheetah.Kafka;
using Cheetah.Kafka.Configuration;
using Cheetah.Kafka.Extensions;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class KafkaInstaller
    {
        public static void InstallKafka(
            this IServiceCollection services,
            IConfigurationRoot configuration
        )
        {
            var kafkaConsumerOptions = configuration.Get<KafkaConsumerConfig>();
            var kafkaProducersOptions = configuration.Get<KafkaProducerConfig>();

            services
                .AddCheetahKafka(
                    configuration,
                    options =>
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
                    }
                )
                // todo: SetErrorHandler && SecurityProtocol
                .WithConsumer<Ignore, string>(x =>
                {
                    x.SetKeyDeserializer(_ => Deserializers.Ignore);
                    x.SetValueDeserializer(_ => Deserializers.Utf8);
                    x.ConfigureClient(cfg => 
                    {
                        cfg.GroupId = kafkaConsumerOptions.ConsumerName;
                    });
                })
                .WithProducer<Null, string>(x =>
                {
                    x.SetValueSerializer(_ => Serializers.Utf8);
                    x.SetKeySerializer(_ => Serializers.Null);
                    x.ConfigureClient(cfg => 
                    {
                        cfg.ClientId = kafkaProducersOptions.ProducerName;
                    });
                });
        }
    }
}
