using System.Net;

namespace Cheetah.WebApi.Core.Config
{
    public class KafkaConsumerConfig
    {
        public const string Position = nameof(KafkaConsumerConfig);
        public string Topic { get; set; } = "InputTopic";
        public string ConsumerName { get; set; } = "test";
    }
}