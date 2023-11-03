using Cheetah.Core.Config;

namespace Cheetah.WebApi.Core.Config
{
    public class WebApiOpenSearchConfig : OpenSearchConfig
    {
        public int PaginationSize { get; set; } = 1000;
        public int ScrollLifetimeSeconds { get; set; } = 60;
        public int MaxBuckets { get; set; } = ushort.MaxValue; //[search.max_buckets] cluster level setting
    }
}