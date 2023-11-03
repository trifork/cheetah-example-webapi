using System;
using System.Threading;
using System.Threading.Tasks;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Cheetah.WebApi
{
    /// <summary>
    /// This class is used to ensure the kafka consumer is subscribed to a topic
    /// </summary>
    class TopicSubscriberService : IHostedService
    {
        private IConsumer<Ignore, string> _kafkaConsumer;
        private IOptions<KafkaConsumerConfig> _kafkaConsumerOptions;

        public TopicSubscriberService(IConsumer<Ignore, string> kafkaConsumer,
         IOptions<KafkaConsumerConfig> kafkaConsumerOptions)
        {
            _kafkaConsumer = kafkaConsumer;
            _kafkaConsumerOptions = kafkaConsumerOptions;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_kafkaConsumerOptions.Value.Topic);
            // NB: If you want to consume messages in a hosted service, then look at the cheetah-example-alertservice repository.
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Close();
            return Task.CompletedTask;
        }
    }
}
