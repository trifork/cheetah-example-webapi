using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cheetah.Kafka;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cheetah.WebApi.Presentation.Controllers
{
    /// <summary>
    /// Simple API collection for kafka which creates a new client for each request+response.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class KafkaController2 : ControllerBase
    {
        private readonly IOptions<KafkaProducerConfig> _kafkaProducerConfig;
        private readonly IOptions<KafkaConsumerConfig> kafkaConsumerConfig;
        private readonly ILogger<KafkaController> _logger;
        private readonly KafkaClientFactory kafkaClientFactory;

        public KafkaController2(
            IOptions<KafkaProducerConfig> kafkaProducerConfig,
            IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
            ILogger<KafkaController> logger,
            KafkaClientFactory kafkaClientFactory
        )
        {
            _kafkaProducerConfig = kafkaProducerConfig;
            this.kafkaConsumerConfig = kafkaConsumerConfig;
            _logger = logger;
            this.kafkaClientFactory = kafkaClientFactory;
        }

        /// <summary>
        /// Consume a message from kafka
        /// </summary>
        /// <returns></returns>
        [HttpGet("consume")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMessage(CancellationToken token)
        {
            var consumerOptions = new ConsumerOptions<Ignore, string>();
            consumerOptions.ConfigureClient(cfg => 
            {
                cfg.GroupId = kafkaConsumerConfig.Value.ConsumerName;
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
            consumerOptions.SetKeyDeserializer(Deserializers.Ignore);
            consumerOptions.SetValueDeserializer(Deserializers.Utf8);
            var kafkaConsumer = kafkaClientFactory
                .CreateConsumerBuilder(consumerOptions)// using kafka in a request-response manner has a large overhead
                .Build();

            kafkaConsumer.Subscribe(kafkaConsumerConfig.Value.Topic);
            using var syncTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            syncTokenSource.CancelAfter(TimeSpan.FromSeconds(10)); // todo: make configurable
            while (!syncTokenSource.IsCancellationRequested) // we can not expect consumergroup member to be ready right away, so lets keep checking for a period of time
            {
                try
                {
                    var consumeResult = kafkaConsumer.Consume(syncTokenSource.Token);
                    if (consumeResult?.Message != null)
                    {
                        kafkaConsumer.Commit(consumeResult); // Commit the offset
                        _logger.LogDebug(
                            $"Received message: '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'."
                        );
                        return Ok(consumeResult.Message.Value);
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // timeout from syncTokenSource
                }
                catch (Exception e)
                {
                    return BadRequest("Received error from Kafka: " + e.Message);
                }
                finally
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    kafkaConsumer.Close();
                }
            }

            return NotFound($"No messages left!");
        }

        /// <summary>
        /// Produce a message to kafka
        /// </summary>
        /// <returns></returns>
        [HttpPost("produce")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProductMessage(string message)
        {
            using var _kafkaProducer = kafkaClientFactory
                .CreateProducerBuilder<Null, string>()
                .Build();
            var msg = await _kafkaProducer.ProduceAsync(
                _kafkaProducerConfig.Value.Topic,
                new Message<Null, string> { Value = message }
            ); // Note: producing synchronously is slow and should generally be avoided.
            _logger.LogDebug($"msg sent at offset: {msg.Offset.Value} for topic: {msg.Topic}");

            return Ok("Msg sent!");
        }
    }
}
