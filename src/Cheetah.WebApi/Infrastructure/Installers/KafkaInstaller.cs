using Microsoft.Extensions.DependencyInjection;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Cheetah.Kafka.Extensions;
using Microsoft.Extensions.Configuration;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    public static class KafkaInstaller
    {
        public static void InstallKafka(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
        {
            var kafkaConsumerOptions = configuration.Get<KafkaConsumerConfig>();
            services.AddCheetahKafka(configuration, options =>
    {
        options.ConfigureDefaultConsumer(config =>
        {
            config.AllowAutoCreateTopics = false;
            config.GroupId = kafkaConsumerOptions.ConsumerName;
            config.AutoOffsetReset = AutoOffsetReset.Latest;
            config.EnableAutoCommit = true;
        });
    })
    // todo: SetErrorHandler && SecurityProtocol
    .WithConsumer<Ignore, string>()
    .WithProducer<string, string>();
        }
    }
}