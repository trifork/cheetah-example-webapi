using System.Net;

namespace Cheetah.WebApi.Core.Config
{
    public class KafkaProducerConfig
    {
        public const string Position = nameof(KafkaProducerConfig);
        public string Topic { get; set; } = "OutputTopic";
        public string ProducerName { get; set; } = Dns.GetHostName();
    }
}
