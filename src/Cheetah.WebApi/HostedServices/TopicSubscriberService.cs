using System;
using System.Threading;
using System.Threading.Tasks;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TopicSubscriberService> _logger;

        public TopicSubscriberService(IConsumer<Ignore, string> kafkaConsumer,
         IOptions<KafkaConsumerConfig> kafkaConsumerOptions, ILogger<TopicSubscriberService> logger)
        {
            _kafkaConsumer = kafkaConsumer;
            _kafkaConsumerOptions = kafkaConsumerOptions;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_kafkaConsumerOptions.Value.Topic);
            _logger.LogDebug("Subscribed to {topic} with consumergroup {groupid}", _kafkaConsumerOptions.Value.Topic, _kafkaConsumer.MemberId);
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
