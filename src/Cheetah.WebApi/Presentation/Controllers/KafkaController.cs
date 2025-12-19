using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cheetah.WebApi.Presentation.Controllers
{
    /// <summary>
    /// Simply API collection to enable consume and produce for kafka messages. Relies on singleton clients.
    /// </summary>
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

        public KafkaController(
            IConsumer<Ignore, string> kafkaConsumer,
            IProducer<Null, string> kafkaProducer,
            IOptions<KafkaProducerConfig> kafkaProducerConfig,
            ILogger<KafkaController> logger
        )
        {
            _kafkaProducerConfig = kafkaProducerConfig;
            _logger = logger;
            _kafkaConsumer = kafkaConsumer; // todo: not thread safe?
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
            var msg = _kafkaConsumer.Consume(TimeSpan.FromMilliseconds(100)); // todo: make configurable
            if (msg?.Message == null)
            {
                _logger.LogDebug("msg info {msgInfo}", msg);
                return NotFound("No messages left!");
            }
            return Ok(msg.Message?.Value);
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
            var msg = await _kafkaProducer.ProduceAsync(
                _kafkaProducerConfig.Value.Topic,
                new Message<Null, string> { Value = message }
            ); // Note: producing synchronously is slow and should generally be avoided.
            return Ok($"msg sent at offset: {msg.Offset.Value} for topic: {msg.Topic}");
        }
    }
}
