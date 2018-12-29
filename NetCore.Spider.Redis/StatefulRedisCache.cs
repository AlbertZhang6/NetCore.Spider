using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;

namespace NetCore.Spider.Redis
{
    internal class StatefulRedisCache : RedisCache
    {
        public int Timestamp
        {
            get;
            set;
        }

        public StatefulRedisCache(IOptions<RedisCacheOptions> options)
            : base(options)
        {
        }
    }
}
