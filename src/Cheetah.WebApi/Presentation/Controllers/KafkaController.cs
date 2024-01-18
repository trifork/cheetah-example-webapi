using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cheetah.WebApi.Core.Config;
using Confluent.Kafka;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IProducer<string, string> _kafkaProducer;
        private readonly IOptions<KafkaProducerConfig> _kafkaProducerConfig;

        public KafkaController(IConsumer<Ignore, string> kafkaConsumer, IOptions<KafkaProducerConfig> kafkaProducerConfig)
        {
            _kafkaProducerConfig = kafkaProducerConfig;
            _kafkaConsumer = kafkaConsumer;
            // _kafkaProducer = kafkaProducer;
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
            Console.WriteLine(_kafkaConsumer.ToString());
            _kafkaConsumer.Subscription.ForEach(
                subscription =>
                    Console.WriteLine($"Subscribed to: {subscription}")
            );
            // Console.WriteLine("Subscribing to topic: {_kafkaConsumerConfig.Value.Topic}");
            Console.WriteLine("Before consume");
            var msg = _kafkaConsumer.Consume(TimeSpan.FromMilliseconds(3000)); // todo: make configurable
            Console.WriteLine("After consume");
            if (msg?.Message == null)
            {
                return NotFound("No messages left!");
            }
            else
            {
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
            var msg = await _kafkaProducer.ProduceAsync(_kafkaProducerConfig.Value.Topic, new Message<string, string> { Value = message });
            return Ok($"msg sent at offset: {msg.Offset.Value} for topic: {msg.Topic}");
        }
    }
}