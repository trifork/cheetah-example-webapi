using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cheetah.WebApi.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class KafkaController : ControllerBase
    {
        private readonly IConsumer<Ignore, string> _kafkaConsumer;
        private readonly IProducer<Null, string> _kafkaProducer;
        private readonly IOptions<KafkaProducerConfig> _kafkaProducerConfig;
        private readonly ILogger<KafkaController> _logger;

        public KafkaController(IConsumer<Ignore, string> kafkaConsumer,
        IProducer<Null, string> kafkaProducer, IOptions<KafkaProducerConfig> kafkaProducerConfig , ILogger<KafkaController> logger)
        {
            _kafkaProducerConfig = kafkaProducerConfig;
            _logger = logger;
            _kafkaConsumer = kafkaConsumer;
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Consume a message from kafka
        /// </summary>
        /// <returns></returns>
        [HttpGet("consume")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMessage()
        {
            _logger.LogDebug("Resuming from {topicPartition}", _kafkaConsumer.Assignment);
            _kafkaConsumer.Resume(_kafkaConsumer.Assignment);
            var msg = _kafkaConsumer.Consume(TimeSpan.FromMilliseconds(100)); // todo: make configurable

            if (msg?.Message == null)
            {
                _kafkaConsumer.Pause(_kafkaConsumer.Assignment);
                _logger.LogDebug("Pausing {topicPartition}", _kafkaConsumer.Assignment);

                return NotFound("No messages left!");
            }
            else
            {
                _kafkaConsumer.Commit(msg); // Commit the offset

                _kafkaConsumer.Pause(_kafkaConsumer.Assignment);
                _logger.LogDebug("Pausing {topicPartition}", _kafkaConsumer.Assignment);

                return Ok(msg.Message?.Value);
            }
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
            var msg = await _kafkaProducer.ProduceAsync(_kafkaProducerConfig.Value.Topic, new Message<Null, string> { Value = message }); // Note: producing synchronously is slow and should generally be avoided.
            return Ok($"msg sent at offset: {msg.Offset.Value} for topic: {msg.Topic}");
        }
    }
}